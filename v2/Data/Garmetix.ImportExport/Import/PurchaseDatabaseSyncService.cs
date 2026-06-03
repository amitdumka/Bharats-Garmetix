using Garmetix.ImportExport.Models;
using System.Text.Json;

namespace Garmetix.ImportExports.Services
{
    public class PurchaseDatabaseSyncService
    {
        // Replace 'GarmetixDbContext' with your actual EF Core context class name
        private readonly GarmetixDbContext _context;

        public PurchaseDatabaseSyncService(GarmetixDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessImportAsync(string jsonFilePath, bool importProducts, bool importStock, bool importInvoices, Guid storeId)
        {
            if (!File.Exists(jsonFilePath)) throw new FileNotFoundException("Backup JSON not found.");

            string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var rawData = JsonSerializer.Deserialize<List<ExcelPurchaseRow>>(jsonContent);

            if (rawData == null || !rawData.Any()) return true;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. IMPORT PRODUCTS
                if (importProducts)
                {
                    await SyncProductsAsync(rawData);
                }

                // 2. IMPORT STOCK
                if (importStock)
                {
                    await SyncStockAsync(rawData, storeId);
                }

                // 3. IMPORT PURCHASE INVOICES (And Vendors)
                if (importInvoices)
                {
                    await SyncPurchaseInvoicesAsync(rawData, storeId);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                // Log exception here
                throw new Exception($"Database sync failed: {ex.Message}", ex);
            }
        }

        private async Task SyncProductsAsync(List<ExcelPurchaseRow> rawData)
        {
            // Get unique products from Excel
            var uniqueProducts = rawData
                .GroupBy(x => x.Barcode)
                .Select(g => g.First())
                .ToList();

            var importedBarcodes = uniqueProducts.Select(p => p.Barcode).ToList();

            // Single network trip to find existing products
            var existingProducts = await _context.Products
                .Where(p => importedBarcodes.Contains(p.Barcode))
                .ToDictionaryAsync(p => p.Barcode);

            var newProducts = new List<Product>();

            // TODO: In a real app, you would also bulk-fetch or create ProductCategories and UOMs here.
            // For safety, assuming a default category exists for the fallback.
            var defaultCategoryId = await _context.ProductCategories.Select(c => c.Id).FirstOrDefaultAsync();

            foreach (var item in uniqueProducts)
            {
                if (!existingProducts.ContainsKey(item.Barcode))
                {
                    newProducts.Add(new Product
                    {
                        Id = Guid.NewGuid(),
                        Barcode = item.Barcode,
                        Name = item.ItemName,
                        Descriptions = item.Description,
                        MRP = item.MRP,
                        TaxRate = item.InputTaxRate,
                        ProductCategoryId = defaultCategoryId,
                        // Add standard defaults based on your models
                        CompanyId = Guid.Empty // Set to actual CompanyId
                    });
                }
                else
                {
                    // Update existing product if MRP changed
                    var prod = existingProducts[item.Barcode];
                    prod.MRP = item.MRP;
                    prod.TaxRate = item.InputTaxRate;
                }
            }

            if (newProducts.Any())
            {
                await _context.Products.AddRangeAsync(newProducts);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SyncStockAsync(List<ExcelPurchaseRow> rawData, Guid storeId)
        {
            var consolidatedStock = rawData
                .GroupBy(x => x.Barcode)
                .Select(g => new
                {
                    Barcode = g.Key,
                    TotalPurchasedQty = g.Sum(i => i.Qty),
                    CostPrice = g.Average(i => i.ActualCost), // Average cost for the batch
                    MRP = g.First().MRP,
                    TaxRate = g.First().InputTaxRate
                }).ToList();

            var barcodes = consolidatedStock.Select(c => c.Barcode).ToList();

            // Get product IDs for the barcodes we just ensured exist in SyncProductsAsync
            var productMap = await _context.Products
                .Where(p => barcodes.Contains(p.Barcode))
                .ToDictionaryAsync(p => p.Barcode, p => p.Id);

            // Fetch existing stock for THIS store
            var existingStocks = await _context.Stocks
                .Where(s => s.StoreId == storeId && barcodes.Contains(s.Barcode))
                .ToDictionaryAsync(s => s.Barcode);

            var newStocks = new List<Stock>();

            foreach (var stockItem in consolidatedStock)
            {
                if (!productMap.ContainsKey(stockItem.Barcode)) continue; // Failsafe

                if (existingStocks.TryGetValue(stockItem.Barcode, out var currentStock))
                {
                    // Update existing stock
                    currentStock.PurchaseQty += stockItem.TotalPurchasedQty;
                    currentStock.CostPrice = stockItem.CostPrice; // Update to latest cost price
                    currentStock.MRP = stockItem.MRP;
                }
                else
                {
                    // Create new stock entry for this store
                    newStocks.Add(new Stock
                    {
                        Id = Guid.NewGuid(),
                        StoreId = storeId,
                        ProductId = productMap[stockItem.Barcode],
                        Barcode = stockItem.Barcode,
                        PurchaseQty = stockItem.TotalPurchasedQty,
                        CostPrice = stockItem.CostPrice,
                        MRP = stockItem.MRP,
                        TaxRate = stockItem.TaxRate
                    });
                }
            }

            if (newStocks.Any())
            {
                await _context.Stocks.AddRangeAsync(newStocks);
            }

            await _context.SaveChangesAsync();
        }

        private async Task SyncPurchaseInvoicesAsync(List<ExcelPurchaseRow> rawData, Guid storeId)
        {
            // Group by Invoice Number (and Supplier)
            var invoiceGroups = rawData.GroupBy(r => new { r.InwardNumber, r.Supplier }).ToList();

            // Fetch product IDs for Invoice Items
            var allBarcodes = rawData.Select(r => r.Barcode).Distinct().ToList();
            var productMap = await _context.Products
                .Where(p => allBarcodes.Contains(p.Barcode))
                .ToDictionaryAsync(p => p.Barcode, p => p.Id);

            foreach (var group in invoiceGroups)
            {
                string inwardNo = group.Key.InwardNumber;
                string vendorName = group.Key.Supplier;

                // 1. Resolve Vendor
                var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.Name == vendorName);
                if (vendor == null)
                {
                    vendor = new Vendor
                    {
                        Id = Guid.NewGuid(),
                        Name = string.IsNullOrWhiteSpace(vendorName) ? "Unknown Vendor" : vendorName,
                        Address = "Imported",
                        City = "Imported",
                        MobileNumber = "0000000000",
                        CompanyId = Guid.Empty // Set your company ID
                    };
                    await _context.Vendors.AddAsync(vendor);
                    await _context.SaveChangesAsync();
                }

                // 2. Prevent duplicate invoices
                bool invoiceExists = await _context.PurchaseInvoices.AnyAsync(i => i.InwardNumber == inwardNo && i.VendorId == vendor.Id);
                if (invoiceExists) continue; // Skip if already imported

                // 3. Create Invoice Header
                var purchaseInvoice = new PurchaseInvoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = group.First().InvoiceNumber ?? $"GEN-{inwardNo}",
                    InwardNumber = inwardNo,
                    VendorId = vendor.Id,
                    VendorName = vendor.Name,
                    InwardDate = DateTime.TryParse(group.First().InwardDate, out var inDate) ? inDate : DateTime.Now,
                    OnDate = DateTime.TryParse(group.First().InvoiceDate, out var invDate) ? invDate : DateTime.Now,
                    BillAmount = group.Sum(i => i.CostWithAllInclusive),
                    NetAmount = group.Sum(i => i.BasicAmount),
                    TaxAmount = group.Sum(i => i.InputTaxAmount),
                    DiscountAmount = group.Sum(i => i.Discount),
                    ItemCount = group.Count(),
                    Quantity = group.Sum(i => i.Qty)
                };

                await _context.PurchaseInvoices.AddAsync(purchaseInvoice);

                // 4. Create Invoice Items
                var invoiceItems = new List<PurchaseInvoiceItem>();
                foreach (var excelItem in group)
                {
                    if (!productMap.ContainsKey(excelItem.Barcode)) continue;

                    invoiceItems.Add(new PurchaseInvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = purchaseInvoice.Id,
                        ProductId = productMap[excelItem.Barcode],
                        Barcode = excelItem.Barcode,
                        MRP = excelItem.MRP,
                        BasePrice = excelItem.BasicRate,
                        BilledQuantity = excelItem.Qty,
                        DiscountAmount = excelItem.Discount,
                        TaxPercentage = excelItem.InputTaxRate,
                        TaxAmount = excelItem.InputTaxAmount,
                        Amount = excelItem.CostWithAllInclusive // Total cost for this line
                    });
                }

                await _context.PurchaseInvoiceItems.AddRangeAsync(invoiceItems);
            }

            await _context.SaveChangesAsync();
        }
    }
}