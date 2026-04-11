using Garmetix.AI.Billing.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garmetix.Billing.AIBased.Helpers
{
    internal class DatabaseHelper
    {
        public static string DatabaseName => "aadwikabilling_v2.db3";
        public static string DatabasePath => GetDatabasePath();
        public static   SQLiteAsyncConnection _database;
        public static string GetDatabasePath()
        {
            return System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, DatabaseName);
        }
        public static bool IsDatabaseInitialized(string dbPath)
        {
            return System.IO.File.Exists(dbPath);
        }

        public static async Task<bool> IsDatabaseEmpty()
        {
            if (!IsDatabaseInitialized(DatabasePath))
                return true;
            var db = new SQLiteAsyncConnection(DatabasePath);
            var productCount = await db.Table<Product>().CountAsync();
            return productCount == 0;
        }
        public static async Task ClearDatabase()
        {
            if (IsDatabaseInitialized(DatabasePath))
            {
                var db = new SQLiteAsyncConnection(DatabasePath);
                await db.DeleteAllAsync<Invoice>();
                await db.DeleteAllAsync<InvoiceItem>();
                await db.DeleteAllAsync<Product>();
                await db.DeleteAllAsync<Customer>();
                await db.DeleteAllAsync<PaymentDetail>();
            }
        }

        public static async Task ResetDatabase()
        {
            await ClearDatabase();
            DeleteDatabase();
            await InitializeDatabaseAsync();
        }
        public static bool DoesDatabaseExist()
        {
            return IsDatabaseInitialized(DatabasePath);
        }
        public static bool IsDatabaseEmptySync()
        {
            if (!IsDatabaseInitialized(DatabasePath))
                return true;
            var db = new SQLiteConnection(DatabasePath);
            var productCount = db.Table<Product>().Count();
            productCount+= db.Table<Invoice>().Count();
            return productCount == 0;
        }
        public static void DeleteDatabase()
        {
            if (IsDatabaseInitialized(DatabasePath))
            {
                System.IO.File.Delete(DatabasePath);
                _database = null;
            }
        }

        public static SQLiteAsyncConnection GetDatabase()
        {
            if (_database == null)
            {
                throw new InvalidOperationException("Database not initialized. Call InitializeDatabaseAsync() first.");
            }
            return _database;
        }


        public static async Task<SQLiteAsyncConnection> InitializeDatabaseAsync()
        {
            if (!IsDatabaseInitialized(DatabasePath))
            {
                // Create an empty file to initialize the database
                using (var stream = System.IO.File.Create(DatabasePath)) { }
            }
            if(!IsDatabaseEmptySync())
            {
                return GetDatabase();
            }
            _database = new SQLiteAsyncConnection(DatabasePath);

            await _database.CreateTableAsync<Invoice>();
            await _database.CreateTableAsync<InvoiceItem>();
            await _database.CreateTableAsync<Product>();
            await _database.CreateTableAsync<Customer>();
            await _database.CreateTableAsync<PaymentDetail>();


            // Load entire product catalog into background memory ONCE
            var allProducts = await _database.Table<Product>().ToListAsync();

            // If DB is empty, add dummy data matching the new schema
            if (!allProducts.Any())
            {
                var dummy1 = new Product { Barcode = "1001", Name = "Cotton Kurta", BaseRate = 1500, Category = GarmentCategory.ReadyMade, CurrentQty = 50, Unit = "Pcs", TaxRate = 5m };
                var dummy2 = new Product { Barcode = "1002", Name = "Silk Saree Fabric", BaseRate = 3000, Category = GarmentCategory.Fabric, CurrentQty = 100, Unit = "Mtr", TaxRate = 5m };
                var dummy3 = new Product { Barcode = "1003", Name = "Zari Lace", BaseRate = 200, Category = GarmentCategory.Accessories, CurrentQty = 200, Unit = "Mtr", TaxRate = 5m };
                var dummy4 = new Product { Barcode = "1004", Name = "Embroidered Blouse Piece", BaseRate = 1200, Category = GarmentCategory.Fabric, CurrentQty = 80, Unit = "Pcs", TaxRate = 5m };
                var dummy5 = new Product { Barcode = "1005", Name = "Designer Dupatta", BaseRate = 800, Category = GarmentCategory.ReadyMade, CurrentQty = 60, Unit = "Pcs", TaxRate = 5m };
                await _database.InsertAsync(dummy1);
                await _database.InsertAsync(dummy2);
                await _database.InsertAsync(dummy3);
                await _database.InsertAsync(dummy4);
                await _database.InsertAsync(dummy5);

                
            }
            return _database;
        }

    }
}
