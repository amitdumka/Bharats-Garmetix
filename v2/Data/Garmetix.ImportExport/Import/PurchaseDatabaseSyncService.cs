using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.ImportExport.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Garmetix.ImportExports.Services
{
    public class PurchaseDatabaseSyncService
    {
        // Replace 'GarmetixDbContext' with your actual EF Core context class name
        private readonly DatabaseContext _context;

        private readonly CategoryMappingService _categoryService;

        public PurchaseDatabaseSyncService(DatabaseContext context, CategoryMappingService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task<bool> ProcessImportAsync(string jsonFilePath, bool importProducts, bool importStock, bool importInvoices, Guid storeId)
        {
            if (!File.Exists(jsonFilePath)) throw new FileNotFoundException("Backup JSON not found.");

            string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var rawData = JsonSerializer.Deserialize<List<ExcelPurchaseRow>>(jsonContent);

            if (rawData == null || !rawData.Any()) return true;

            using var transaction = await _context.Database.BeginTransactionAsync();

            await _categoryService.InitializeCacheAsync(DatabaseService.CompanyId);
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
            // 1. Get unique products from Excel
            var uniqueProducts = rawData
                .GroupBy(x => x.Barcode)
                .Select(g => g.First())
                .ToList();

            var importedBarcodes = uniqueProducts.Select(p => p.Barcode).ToList();

            // 2. Single network trip to find existing products
            var existingProducts = await _context.Products
                .Where(p => importedBarcodes.Contains(p.Barcode))
                .ToDictionaryAsync(p => p.Barcode);

            var newProducts = new List<Product>();

            // 3. Process each item using the decoupled method
            foreach (var item in uniqueProducts)
            {
                // Try to get the existing product, it will be null if it doesn't exist
                existingProducts.TryGetValue(item.Barcode, out var existingProduct);

                // Process the single item
                var processedProduct = await SyncProductAsync(item, existingProduct);

                // If the method returns a product, it means it's a NEW product that needs to be added
                if (processedProduct != null)
                {
                    newProducts.Add(processedProduct);
                }
            }

            // 4. Batch insert and save
            if (newProducts.Any())
            {
                await _context.Products.AddRangeAsync(newProducts);
            }

            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Processes a single product. Returns a new Product if creation is required, or null if an existing product was updated.
        /// </summary>
        private async Task<Product?> SyncProductAsync(ExcelPurchaseRow item, Product? existingProduct)
        {
            // UPDATE LOGIC
            if (existingProduct != null)
            {
                // Update existing product if MRP changed
                existingProduct.MRP = item.MRP;
                existingProduct.TaxRate = item.InputTaxRate;

                // Return null so the orchestrator knows not to add this to the "newProducts" list
                return null;
            }

            // CREATION LOGIC
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Barcode = item.Barcode, //TODO: new Barcode System
                Name = item.ItemName,
                Descriptions = item.Description,
                MRP = item.MRP,
                TaxRate = item.MRPOutputTaxRate,
                TaxType = TaxType.GST, // Assuming GST for all products, adjust as needed

                ProductGroup = ProductGroup.Others,
                ProductType = ProductType.Readymade,
                Unit = Unit.NoUnit,

                HSNCode = item.HSNCode,

                CompanyId = DatabaseService.CompanyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DatabaseService.Instance.CurrentUser?.UserName ?? "System",
                Deleted = false,
                Synced = false,
                StoreGroupId = DatabaseService.StoreGroupId,
                UpdatedAt = DateTime.UtcNow,

                Stocks = new List<Stock>
        {
            new Stock
            {
                Id = Guid.NewGuid(),
                Barcode = item.Barcode,
                MRP = item.MRP,
                TaxRate = item.MRPOutputTaxRate,
                HSNCode = item.HSNCode,
                TaxType = TaxType.GST,

                // Cost price basic rate + plus amount, plus Supplier margin
                CostPrice = CalculateCostPrice(item.BasicRate, item.InputTaxRate, item.AdditionalCostRate),
                BrandedProduct = false,
                StockType = StockType.Others,
                TaxId = await GetTaxId(item.MRPOutputTaxRate),

                Unit = Unit.NoUnit,

                CompanyId = DatabaseService.CompanyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DatabaseService.Instance.CurrentUser?.UserName ?? "System",
                Deleted = false,
                StoreGroupId = DatabaseService.StoreGroupId,
                StoreId = DatabaseService.StoreId,
                UpdatedAt = DateTime.UtcNow,
                SoldQty = 0,
                PurchaseQty = 0,
                SoldValue = 0,
                Synced = false
            }
        }
            };

            // Apply unit syncing and category mappings
            newProduct.Stocks.First().Unit = newProduct.Unit;
            newProduct = await SetCategoryAndUnit(newProduct, item.Category);

            return newProduct;
        }

        [Obsolete]
        private async Task SyncProductsAsync_old(List<ExcelPurchaseRow> rawData)
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
                    var newPoduct = new Product
                    {
                        Id = Guid.NewGuid(),
                        Barcode = item.Barcode, //TODO: new Barcode System
                        Name = item.ItemName,
                        Descriptions = item.Description,
                        MRP = item.MRP,
                        TaxRate = item.MRPOutputTaxRate,
                        TaxType = TaxType.GST, // Assuming GST for all products, adjust as needed

                        //TODO: create method to handle this
                        // ProductSubCategoryId = defaultCategoryId, // Set to actual subcategory if you have logic to determine it
                        //ProductCategoryId = defaultCategoryId,
                        ProductGroup = ProductGroup.Others,
                        ProductType = ProductType.Readymade,
                        Unit = Unit.NoUnit,

                        HSNCode = item.HSNCode,

                        // Add standard defaults based on your models
                        CompanyId = DatabaseService.CompanyId, // Set to actual CompanyId
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DatabaseService.Instance.CurrentUser?.UserName ?? "System",
                        Deleted = false,
                        Synced = false,
                        StoreGroupId = DatabaseService.StoreGroupId,
                        UpdatedAt = DateTime.UtcNow,
                        //TODO: Implement proper UOM and ProductType mapping based on your actual data and models
                        Stocks = new List<Stock> {
                            new Stock {
                                Id = Guid.NewGuid(),
                                Barcode = item.Barcode,

                                MRP=item.MRP,
                                TaxRate=item.MRPOutputTaxRate,
                                HSNCode=item.HSNCode,
                                TaxType=TaxType.GST,

                                // Cost price basic rate+plus amount, plus Supplier margin
                                CostPrice=CalculateCostPrice(item.BasicRate, item.InputTaxRate, item.AdditionalCostRate),
                                BrandedProduct=false,
                                StockType=StockType.Others,
                                TaxId= await GetTaxId(item.MRPOutputTaxRate),

                                Unit=Unit.NoUnit,

                                CompanyId = DatabaseService.CompanyId,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = DatabaseService.Instance.CurrentUser?.UserName ?? "System",
                                Deleted = false,
                                StoreGroupId = DatabaseService.StoreGroupId,
                                StoreId=DatabaseService.StoreId, UpdatedAt = DateTime.UtcNow,
                                SoldQty=0, PurchaseQty=0
                                ,SoldValue=0, Synced=false,
                        }, }
                    };
                    newPoduct.Stocks.First().Unit = newPoduct.Unit;
                    newPoduct = await SetCategoryAndUnit(newPoduct, item.Category);
                    newProducts.Add(newPoduct);
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

        private decimal CalculateCostPrice(decimal basicRate, decimal TaxRate, decimal supplierRate)
        {
            //Adding Tax
            var costPrice = (basicRate * (TaxRate / 100)) + basicRate;
            //Add Supplier Tax
            if (supplierRate > 0)
            {
                costPrice += (costPrice * (supplierRate / 100));
            }

            return costPrice;
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
                        MobileNumber = "0000000000", Active = true, BillAmount=0m, BillCount=0,
                        CreatedAt = DateTime.Now, CreatedBy=DatabaseService.Instance.CurrentUser.Name,
                        Deleted=false, Paid=9, Synced=false, 
                        UpdatedAt=DateTime.Now,
                        
                        CompanyId = DatabaseService.CompanyId,
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
                    Quantity = group.Sum(i => i.Qty) , CompanyId=DatabaseService.CompanyId, CreatedAt=DateTime.UtcNow,
                    CreatedBy=DatabaseService.Instance.CurrentUser.UserName, 
                    Deleted=false, DueDate=DateTime.UtcNow, InterState=true,
                    InvoiceStatus=InvoiceStatus.Draft, InvoiceType=InvoiceType.Regular, 
                    Synced=false, UpdatedAt=DateTime.UtcNow,
                    PaymentMode=PaymentMode.Others, RoundOff=0, ReturnInvoice=false,                    
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
                        ,
                        Synced = false,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                        TaxType = TaxType.IGST, Deleted = false, CompanyId=DatabaseService.CompanyId, 
                        TaxId= await GetTaxId(excelItem.MRPOutputTaxRate), 
                    });
                }

                await _context.PurchaseInvoiceItems.AddRangeAsync(invoiceItems);
            }

            await _context.SaveChangesAsync();
        }

        //TODO: implements
        private Dictionary<decimal, Guid> TaxRateId = new Dictionary<decimal, Guid>();

        private async Task<Guid> GetTaxId(decimal taxRate)
        {
            Guid? guid = TaxRateId.FirstOrDefault(x => x.Key == taxRate).Value;
            if (guid == null)
            {
                guid = (await _context.Taxes.FirstOrDefaultAsync(x => x.CompositeRate == taxRate))?.Id;
                if (guid == null)
                {
                    var tax = new Tax
                    {
                        CompositeRate = taxRate,
                        CreatedAt = DateTime.UtcNow,
                        Deleted = false,
                        Id = Guid.NewGuid(),
                        Name = $"GST {taxRate}% ",
                        Synced = false,
                        UpdatedAt = DateTime.UtcNow,
                        TaxType = TaxType.GST,
                    };
                    guid = tax.Id;
                    await _context.AddAsync(tax);
                    await _context.SaveChangesAsync();
                }
                TaxRateId.Add(taxRate, guid.Value);
            }
            return guid.Value;
        }

        private async Task<Guid> GetProductType()
        { return Guid.Empty; }

        private async Task<Unit> GetUOM()
        { return Unit.Pcs; }

        private async Task<Guid> GetProductCategory()
        { return Guid.Empty; }

        private async Task<Guid> GetSubCategory()
        { return Guid.Empty; }

        private async Task<string> GenerateBarcode(DateTime inwardDate, string brandCode, int LastCount, Guid storeid)
        {
            // TODO:Implement your barcode generation logic here based on your requirements
            // For example, you could concatenate item name, style code, and shade color with a unique identifier
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            return $"";
        }

        //private async Task<Product> SetCategoryAndUnit(Product product, string category)
        //{
        //    // based on Category Set
        //    Guid catgoryid;
        //    Guid subCategoryid;
        //    ProductGroup grp;
        //    ProductType ptype;
        //    //TODO: create method to handle this
        //    //TODO: if productCategory and Sub Category not fount then create it
        //    // Cache this data so need to call db again again

        //    product.ProductSubCategoryId = subCategoryid; // Set to actual subcategory if you have logic to determine it
        //    product.ProductCategoryId = catgoryid;

        //    product.ProductGroup = grp; // ProductGroup.Others;
        //    product.ProductType = ptype; // ProductType.Readmade;

        //    product.Unit = Unit.NoUnit;

        //    return product;

        //}
        private async Task<Product> SetCategoryAndUnit(Product product, string rawCategoryName)
        {
            if (string.IsNullOrWhiteSpace(rawCategoryName))
                rawCategoryName = "MiscItem"; // Default fallback

            // 1. Setup Defaults
            string parentCategoryName = rawCategoryName; // Maps to DB ProductCategory
            string subCategoryName = string.Empty;       // Maps to DB ProductSubCategory
            ProductGroup grp = ProductGroup.Others;
            ProductType ptype = ProductType.Others;
            Unit unit = Unit.Pcs;

            // 2. Map Excel Table Data using a clean switch statement
            switch (rawCategoryName.Trim().ToLower())
            {
                // --- SUITS & BLAZERS ---
                case "suit":
                    grp = ProductGroup.Suits; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "blazers":
                    grp = ProductGroup.Blazers; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "five pcs suits":
                    parentCategoryName = "Suit"; subCategoryName = "Five Pcs Suits";
                    grp = ProductGroup.Suits; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "three pcs suits":
                    parentCategoryName = "Suit"; subCategoryName = "Three Pcs Suits";
                    grp = ProductGroup.Suits; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                // --- ETHNIC WEAR ---
                case "sherwani":
                    grp = ProductGroup.Sherwani; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "indo western":
                    grp = ProductGroup.Sherwani; ptype = ProductType.Readymade; unit = Unit.Pcs; // Or create IndoWestern group
                    break;

                case "kurta":
                    grp = ProductGroup.Kurta; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "kurta pajama":
                    grp = ProductGroup.KurtaPajama; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "pajama":
                    grp = ProductGroup.Pajama; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "koti set":
                case "bundi":
                    grp = ProductGroup.Kurta; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                // --- CASUAL / FORMAL WEAR ---
                case "shirt":
                    grp = ProductGroup.Shirting; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "t-shirt":
                    grp = ProductGroup.Readymade; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "roundneck":
                    parentCategoryName = "T-Shirt"; subCategoryName = "RoundNeck";
                    grp = ProductGroup.Readymade; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "pant":
                case "jeans":
                case "shorts":
                    grp = ProductGroup.Readymade; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                // --- WEDDING ACCESSORIES ---
                case "dupatta":
                    grp = ProductGroup.Dupatta; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "pagadi":
                    grp = ProductGroup.Pagadi; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "pagadi dupatta":
                    grp = ProductGroup.PagadiDupattaSet; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "mens mala":
                case "mala":
                    grp = ProductGroup.Accessories; ptype = ProductType.Accessories; unit = Unit.Pcs;
                    break;

                case "broch":
                    grp = ProductGroup.Brochs; ptype = ProductType.Accessories; unit = Unit.Pcs;
                    break;

                case "sword":
                case "pocket square":
                case "tie":
                case "thali":
                case "kalangi":
                    grp = ProductGroup.Accessories; ptype = ProductType.Accessories; unit = Unit.Pcs;
                    break;

                // --- FOOTWEAR ---
                case "nagra jutti":
                    grp = ProductGroup.Nagra; ptype = ProductType.Shoes; unit = Unit.Pcs;
                    break;

                case "shoes":
                    grp = ProductGroup.Shoes; ptype = ProductType.Shoes; unit = Unit.Pcs;
                    break;

                // --- JHODPURI ---
                case "jhodpuri":
                    grp = ProductGroup.Jodhpuri; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "jodhpuri blazer":
                    parentCategoryName = "Jhodpuri"; subCategoryName = "Jodhpuri Blazer";
                    grp = ProductGroup.Jodhpuri; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                case "jodhpuri suit":
                    parentCategoryName = "Jhodpuri"; subCategoryName = "Jodhpuri Suit";
                    grp = ProductGroup.Jodhpuri; ptype = ProductType.Readymade; unit = Unit.Pcs;
                    break;

                // --- FABRICS ---
                case "shirting":
                    grp = ProductGroup.Shirting; ptype = ProductType.Fabric; unit = Unit.Meters;
                    break;

                case "suiting":
                    grp = ProductGroup.Suiting; ptype = ProductType.Fabric; unit = Unit.Meters;
                    break;

                // --- DEFAULT ---
                case "miscitem":
                default:
                    grp = ProductGroup.Others; ptype = ProductType.Others; unit = Unit.Pcs;
                    break;
            }

            // 3. Resolve Database IDs using the Cache Service
            Guid categoryId = await _categoryService.GetOrCreateCategoryAsync(parentCategoryName);
            Guid subCategoryId = await _categoryService.GetOrCreateSubCategoryAsync(subCategoryName);

            // 4. Assign properties to the Product
            product.ProductCategoryId = categoryId;

            // Only assign SubCategory if it actually resolved to one
            if (subCategoryId != Guid.Empty)
            {
                product.ProductSubCategoryId = subCategoryId;
            }

            product.ProductGroup = grp;
            product.ProductType = ptype;
            product.Unit = unit;

            return product;
        }
    }
}