using System;
using System.Threading.Tasks;
using SQLite;
using Microsoft.Maui.Controls;
using PointOfSale.Helpers;

namespace PointOfSale.Models
{
    public class DatabaseInit
    {
        private readonly string dbPath;

        public DatabaseInit(string databasePath)
        {
            dbPath = databasePath;
        }

        public async Task InitializeDatabaseAsync()
        {
            var db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Bharat.Inventory.Product>();
            await db.CreateTableAsync<Bharat.Inventory.Stock>();
            await db.CreateTableAsync<Bharat.Invoicing.Customer>();
            await db.CreateTableAsync<PointOfSale.Models.PaymentDetail>();
            await db.CreateTableAsync<PointOfSale.Models.SaleInvoice>();
            await db.CreateTableAsync<PointOfSale.Models.PurchaseInvoice>();
            // ... add others as needed
        }

        public async Task SaveProductAsync(Bharat.Inventory.Product p)
        {
            var db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Bharat.Inventory.Product>();
            await db.InsertOrReplaceAsync(p);
        }

        public async Task SaveStockAsync(Bharat.Inventory.Stock s)
        {
            var db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Bharat.Inventory.Stock>();
            await db.InsertOrReplaceAsync(s);
        }

        public async Task SaveCustomerAsync(Bharat.Invoicing.Customer c)
        {
            var db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Bharat.Invoicing.Customer>();
            await db.InsertOrReplaceAsync(c);
        }

        // Convenience methods to show generated forms. Caller must provide an INavigation instance.
        public async Task ShowProductFormAsync(INavigation navigation)
        {
            var product = new Bharat.Inventory.Product();
            var page = FormGenerator.BuildPage(product, "Edit Product", async p => await SaveProductAsync(p));
            await navigation.PushAsync(page);
        }

        public async Task ShowStockFormAsync(INavigation navigation)
        {
            var stock = new Bharat.Inventory.Stock();
            var page = FormGenerator.BuildPage(stock, "Edit Stock", async s => await SaveStockAsync(s));
            await navigation.PushAsync(page);
        }

        public async Task ShowCustomerFormAsync(INavigation navigation)
        {
            var customer = new Bharat.Invoicing.Customer();
            var page = FormGenerator.BuildPage(customer, "Edit Customer", async c => await SaveCustomerAsync(c));
            await navigation.PushAsync(page);
        }
    }
}








