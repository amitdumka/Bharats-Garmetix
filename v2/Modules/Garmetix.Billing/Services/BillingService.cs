using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Billing.Services
{
    /// <summary>
    /// Billing Service is base of Invoice and Purchase Service.
    /// it will handle this as bases for all service
    /// </summary>
    public partial class BillingService
    {

        public static async Task ShowErrorAsync(string title, Exception ex) => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, $"Error: {ex.Message}", "OK");

        public static async Task ShowErrorAsync(string title, string message) => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, $"Error: {message}", "OK");

        public async Task<Product?> AddOrUpdateProductAsync(Product product, bool update = false)
        {

            if (update)
            {
                var existingProduct = GetContext().Products.Where(x => x.Id == product.Id).FirstOrDefault();
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Barcode = product.Barcode;
                    existingProduct.Descriptions = product.Descriptions;
                    existingProduct.MRP = product.MRP;
                    existingProduct.TaxRate = product.TaxRate;
                    existingProduct.Unit = product.Unit;
                    existingProduct.TaxType = product.TaxType;
                    existingProduct.ProductType = product.ProductType;
                    existingProduct.ProductCategoryId = product.ProductCategoryId;
                    existingProduct.ProductSubCategoryId = product.ProductSubCategoryId;
                    GetContext().Products.Update(existingProduct);
                    await GetContext().SaveChangesAsync();
                    return existingProduct;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                GetContext().Products.Add(product);
                await GetContext().SaveChangesAsync();
            }
            return product;

        }


        public async Task<Stock?> AddStockAsync(Stock stock)
        {
            GetContext().Stocks.Add(stock);
            var result = await GetContext().SaveChangesAsync();

            if (result > 0)
                return stock;
            else return null;
        }

        public static DatabaseContext GetContext() => DatabaseService.Instance.LocalDB;

        public static bool RemoveProduct(Product product, bool delete = false)
        {
            if (delete)
            {
                GetContext().Products.Remove(product);
                return GetContext().SaveChanges() > 0;

            }
            else
            {
                product.Deleted = true;
                GetContext().Products.Update(product);
                return GetContext().SaveChanges() > 0;
            }
        }

        public static async Task<bool> RemoveProduct(Guid companyId, string barcode, bool delete = false)
        {
            if (delete)
            {
                GetContext().Products.RemoveRange(GetContext().Products.Where(x => x.CompanyId == companyId && x.Barcode == barcode));
                return GetContext().SaveChanges() > 0;
            }
            else
            {
                var products = await GetContext().Products.Where(x => x.CompanyId == companyId && x.Barcode == barcode).FirstOrDefaultAsync();
                if (products != null)
                {
                    products.Deleted = true;
                    GetContext().Products.Update(products);
                    return GetContext().SaveChanges() > 0;
                }
            }

            return false;
        }

        public static bool RemoveStock(Stock stock, bool delete = false)
        {
            if (delete)
            {
                GetContext().Stocks.Remove(stock);
                return GetContext().SaveChanges() > 0;

            }
            else
            {
                stock.Deleted = true;
                GetContext().Stocks.Update(stock);
                return GetContext().SaveChanges() > 0;
            }


        }

        public static Guid GetTaxIdByType(TaxType type, decimal percentage, bool output = true)
        {
            if (output)
            {
                var tax = GetContext().Taxes.Where(x => x.TaxType == type && x.CompositeRate == percentage).FirstOrDefault();
                return tax != null ? tax.Id : Guid.Empty;
            }
            else
            {
                var tax = GetContext().Taxes.Where(x => x.TaxType == type && x.CompositeRate == percentage).FirstOrDefault();
                return tax != null ? tax.Id : Guid.Empty;
            }
        }

        public static Tax AddOrUpdateTax(Tax tax, bool update = false)
        {
            if (update)
            {
                var existingTax = GetContext().Taxes.Where(x => x.Id == tax.Id).FirstOrDefault();
                if (existingTax != null)
                {
                    existingTax.TaxType = tax.TaxType;
                    existingTax.CompositeRate = tax.CompositeRate;
                    GetContext().Taxes.Update(existingTax);
                    GetContext().SaveChanges();
                    return existingTax;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                GetContext().Taxes.Add(tax);
                GetContext().SaveChanges();
                return tax;
            }
        }

        /// <summary>
        /// Get Customer Guid
        /// </summary>
        /// <param name="customerMobile"></param>
        /// <returns></returns>

        public static async Task<Guid> GetCustomerIdOrDefaultAsync(string customerMobile)
        {
            var customer = await GetContext().Customers.Where(x => x.MobileNumber == customerMobile).FirstOrDefaultAsync();
            if (customer != null)
            {
                return customer.Id;
            }
            else
            {
                return await BillingService.GetDefaultCustomerAsync();
            }
        }

        /// <summary>
        /// Get Default Customer for walk in customer
        /// </summary>
        /// <returns></returns>
        public static async Task<Guid> GetDefaultCustomerAsync()
        {
            var customer = await GetContext().Customers.Where(x => x.Name == "Walkin Customer").FirstOrDefaultAsync();
            if (customer == null)
            {
                customer = new Customer
                {
                    Name = "Walkin Customer",
                    MobileNumber = "0000000000",
                    Registred = false,
                    Amount = 0,
                    CompanyId = DatabaseService.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.UserName,
                    Deleted = false,
                    Email = "amit.dumka@gmail.com",
                    Id = Guid.NewGuid(),
                    State = "Jharkhand",
                    Synced = false,
                    UpdatedAt = DateTime.UtcNow,
                    BillCount = 0,
                    Address = "Dumka",
                    City = "Dumka",
                    Country = "India",
                    ZipCode = "814101",
                };
                await GetContext().Customers.AddAsync(customer);
                await GetContext().SaveChangesAsync();
            }
            return customer.Id;
        }

        /// <summary>
        /// Remove or delete stock
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Barcode"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public bool RemoveStock(Guid StoreId, string Barcode, bool delete = false)
        { return false; }
        public async Task<bool> UpdateStockRangeAsync(List<InvoiceItem> items, bool sold)
        {
            if (items == null || !items.Any())
                return true;

            var context = GetContext();
            using var trans = await context.Database.BeginTransactionAsync();

            try
            {
                // 1. CONSOLIDATE: Group duplicate barcodes and sum their quantities & amounts.
                // This solves the tracking error by ensuring each barcode only appears once.
                var consolidatedItems = items
                    .GroupBy(i => i.Barcode)
                    .Select(g => new
                    {
                        Barcode = g.Key,
                        TotalBilledQty = g.Sum(i => i.BilledQuantity),
                        // TODO: Update the price value of sold amount. 
                        // Assuming InvoiceItem has a TotalAmount or Price property:
                        TotalSoldValue = g.Sum(i => i.Amount)
                    })
                    .ToList();

                // 2. EXTRACT: Get a simple list of the unique barcodes
                var uniqueBarcodes = consolidatedItems.Select(c => c.Barcode).ToList();

                // 3. BATCH FETCH: Get all matching stock records in a SINGLE database trip. 
                // Solves the N+1 performance issue and uses the centralized DatabaseService.StoreId.
                var stocksToUpdate = await context.Stocks
                    .Where(s => s.StoreId == DatabaseService.StoreId && uniqueBarcodes.Contains(s.Barcode))
                    .ToListAsync();

                // Optional Integrity Check: Did we find stock records for every item on the invoice?
                if (stocksToUpdate.Count != consolidatedItems.Count)
                {
                    // Some barcodes on the invoice don't exist in the Stock table for this store.
                    // You can log this, throw an exception, or let it proceed depending on your business rules.
                }

                // 4. APPLY UPDATES
                foreach (var stock in stocksToUpdate)
                {
                    var billedItem = consolidatedItems.First(c => c.Barcode == stock.Barcode);

                    stock.SoldQty += billedItem.TotalBilledQty;
                    stock.SoldValue += billedItem.TotalSoldValue;

                    // Apply the financial value update
                    // stock.SoldAmount += billedItem.TotalSoldValue; 

                    // NOTE: You DO NOT need to call context.Stocks.Update(stock) here.
                    // Because we fetched these records using EF Core (without .AsNoTracking()), 
                    // EF Core is actively watching them. Changing the property above is enough.
                }

                // 5. SAVE & COMMIT
                // Note: Checking if SaveChangesAsync() == items.Count is dangerous and often wrong 
                // because EF Core batches updates. We just need to ensure it didn't fail.
                await context.SaveChangesAsync();
                await trans.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                await ShowErrorAsync("Stock Update", ex);
                return false;
            }
        }
        public async Task<bool> UpdateStockRangeAsync(List<InvoiceItem> items)
        {
            //TODO: add Try Catch final block in the code
            //TODO: Update the price value of sold amount
            var count = 0;
            var trans = await GetContext().Database.BeginTransactionAsync();
            try
            {
                //TODO: Handle Case , if same barcode or prodcut id multiple entry then it should be added once so no tracking and error update
                foreach (var item in items)
                {
                    //TODO: handle this store id and company id in better way
                    var result = await GetContext().Stocks.Where(x => x.StoreId == DatabaseService.StoreId && x.Barcode == item.Barcode).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        count++;
                        result.SoldQty += item.BilledQuantity;
                        GetContext().Stocks.Update(result);
                    }
                }
                if (count == items.Count)
                {
                    count = await GetContext().SaveChangesAsync();
                    if (count == items.Count)
                    {
                        await trans.CommitAsync();
                        return true;
                    }
                    else
                    {
                        await trans.RollbackAsync();
                        return false;
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                await ShowErrorAsync("Stock Update", ex);
                return false;
            }


        }

        public async Task<bool> UpdateStockRangeAsync(List<PurchaseInvoiceItem> items)
        {
            //TODO: add Try Catch final block in the code
            //TODO: Update the average Price  of Cost price.
            var count = 0;
            var trans = await GetContext().Database.BeginTransactionAsync();
            foreach (var item in items)
            {
                var result = await GetContext().Stocks.Where(x => x.StoreId == DatabaseService.StoreId && x.Barcode == item.Barcode).FirstOrDefaultAsync();
                if (result != null)
                {
                    count++;
                    result.PurchaseQty += item.BilledQuantity;
                    GetContext().Stocks.Update(result);
                }
            }
            if (count == items.Count)
            {
                count = await GetContext().SaveChangesAsync();
                if (count == items.Count)
                {
                    await trans.CommitAsync();
                    return true;
                }
                else
                {
                    await trans.RollbackAsync();
                    return false;
                }
            }
            else return false;
        }

        /// <summary>
        /// Update the stock while purchase or Sale
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="barcode"></param>
        /// <param name="qty"></param>
        /// <param name="sold"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateStockAsync(Guid storeid, string barcode, decimal qty, decimal price, bool sold = false)
        {
            try
            {
                //TODO: handle this store id and company id in better way
                var result = await GetContext().Stocks.Where(x => x.StoreId == DatabaseService.StoreId && x.Barcode == barcode).FirstOrDefaultAsync();

                if (result == null) return false;

                if (sold)
                {
                    // Update the price value of sold amount
                    result.SoldQty += qty;
                }
                else

                {
                    //TODO: Update the average Price  of Cost price.

                    result.PurchaseQty += qty;
                }
                GetContext().Stocks.Update(result);
                return (await GetContext().SaveChangesAsync() > 0);
            }
            catch (Exception)
            {
                //Notify the error
                return false;
            }
        }
    }
}