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
        protected DatabaseContext _localDb => DatabaseService.Instance.LocalDB;

        public static async Task ShowErrorAsync(string title, Exception ex) => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, $"Error: {ex.Message}", "OK");
        public static async Task ShowErrorAsync(string title, string message) => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, $"Error: {message}", "OK");

        public Product AddorUpxdateProduct(Product product, bool update = false)
        { return product; }

        public Stock AddStock(Stock stock)
        { return stock; }

        public DatabaseContext GetContext()
        { return _localDb; }

        public bool RemoveProduct(Product product, bool delete = false)
        { return product != null; }

        public bool RemoveProduct(Guid storeid, string barcode, bool delete = false)
        { return false; }

        public bool RemoveStock(Stock stock, bool delete = false)
        { return false; }

        /// <summary>
        /// Remove or delete stock
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Barcode"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public bool RemoveStock(Guid StoreId, string Barcode, bool delete = false)
        { return false; }


        public async Task<bool> UpdateStockRangeAsync(List<InvoiceItem> items)
        {
            //TODO: add Try Catch final block in the code
            //TODO: Update the price value of sold amount
            var count = 0;
            var trans = await GetContext().Database.BeginTransactionAsync();
            foreach (var item in items)
            {
                var result = await GetContext().Stocks.Where(x => x.StoreId == item.CompanyId && x.Barcode == item.Barcode).FirstOrDefaultAsync();
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
        public async Task<bool> UpdateStockRangeAsync(List<PurchaseInvoiceItem> items)
        {
            //TODO: add Try Catch final block in the code
            //TODO: Update the average Price  of Cost price.
            var count = 0;
            var trans = await GetContext().Database.BeginTransactionAsync();
            foreach (var item in items)
            {
                var result = await GetContext().Stocks.Where(x => x.StoreId == item.CompanyId && x.Barcode == item.Barcode).FirstOrDefaultAsync();
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
        public async Task<bool> UpdateStockAsync(Guid storeid, string barcode, decimal qty, decimal price, bool sold = false)
        {
            try
            {
                var result = await GetContext().Stocks.Where(x => x.StoreId == storeid && x.Barcode == barcode).FirstOrDefaultAsync();

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