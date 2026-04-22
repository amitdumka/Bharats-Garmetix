using Garmetix.AI.Billing.Models;
using Garmetix.Models.Accounting;
using SQLite;
using System.Diagnostics;
using BankTransaction = Garmetix.AI.Billing.Models.BankTransaction;
using ChequeLog = Garmetix.AI.Billing.Models.ChequeLog;

namespace Garmetix.Billing.AIBased.Helpers
{
    public static class DatabaseHelper
    {
        private const string DatabaseFilename = "aadwikabilling_v2.db3";

        // Flags to ensure multi-threaded reading/writing works perfectly in MAUI
        private const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        private static SQLiteAsyncConnection _database;
        private static bool _isInitialized = false;

        // An async lock to prevent multiple ViewModels from trying to initialize the DB at the exact same millisecond
        private static readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public static string DatabasePath => Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, DatabaseFilename);

        /// <summary>
        /// The ONLY method ViewModels should call to get the database. 
        /// It guarantees the database is fully initialized and seeded before returning the connection.
        /// </summary>
        public static async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            if (!_isInitialized)
            {
                await InitializeDatabaseAsync();
            }
            return _database;
        }

        public static async Task InitializeDatabaseAsync()
        {
            // Double-check locking pattern to save performance
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                // Check again inside the lock
                if (_isInitialized) return;

                // SQLite handles file creation automatically. Do not use File.Create()
                _database = new SQLiteAsyncConnection(DatabasePath, Flags);

                // Create tables (Does nothing if they already exist)
                await _database.CreateTableAsync<Invoice>();
                await _database.CreateTableAsync<InvoiceItem>();
                await _database.CreateTableAsync<Product>();
                await _database.CreateTableAsync<Customer>();
                await _database.CreateTableAsync<PaymentDetail>();
                await _database.CreateTableAsync<User>();



                await _database.CreateTableAsync<LedgerGroup>();
                await _database.CreateTableAsync<Ledger>();
                await _database.CreateTableAsync<Party>();
                await _database.CreateTableAsync<Bank>();
                await _database.CreateTableAsync<BankAccount>();
                await _database.CreateTableAsync<Voucher>();
                await _database.CreateTableAsync<CashVoucher>();

                await _database.CreateTableAsync<ChequeLog>();  
                await _database.CreateTableAsync<BankTransaction>();

                // Check for empty DB and seed data
                await SeedDummyDataIfEmptyAsync();

                _isInitialized = true;
                Debug.WriteLine("Garmetix DB: Initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CRITICAL DB ERROR] Failed to initialize database: {ex.Message}");
                throw; // Rethrow to let the app know the database is fundamentally broken
            }
            finally
            {
                _initLock.Release();
            }
        }

        private static async Task SeedDummyDataIfEmptyAsync()
        {
            try
            {
                var productCount = await _database.Table<Product>().CountAsync();

                if (productCount == 0)
                {
                    var dummyData = new[]
                    {
                        new Product { Barcode = "1001", Name = "Cotton Kurta", BaseRate = 1500, Category = GarmentCategory.ReadyMade, CurrentQty = 50, Unit = "Pcs", TaxRate = 5m },
                        new Product { Barcode = "1002", Name = "Silk Saree Fabric", BaseRate = 3000, Category = GarmentCategory.Fabric, CurrentQty = 100, Unit = "Mtr", TaxRate = 5m },
                        new Product { Barcode = "1003", Name = "Zari Lace", BaseRate = 200, Category = GarmentCategory.Accessories, CurrentQty = 200, Unit = "Mtr", TaxRate = 5m },
                        new Product { Barcode = "1004", Name = "Embroidered Blouse Piece", BaseRate = 1200, Category = GarmentCategory.Fabric, CurrentQty = 80, Unit = "Pcs", TaxRate = 5m },
                        new Product { Barcode = "1005", Name = "Designer Dupatta", BaseRate = 800, Category = GarmentCategory.ReadyMade, CurrentQty = 60, Unit = "Pcs", TaxRate = 5m }
                    };

                    await _database.InsertAllAsync(dummyData);
                    Debug.WriteLine("Garmetix DB: Dummy data seeded successfully.");
                }
                var userCount = await _database.Table<User>().CountAsync();
                if (userCount == 0)
                {
                    var admin = new User
                    {
                        Username = "admin",
                        // Hash for password "admin123"
                        PasswordHash = Garmetix.AI.Billing.Services.AuthService.Instance.HashString("admin123")
                    };
                    await _database.InsertAsync(admin);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DB SEED ERROR] Failed to seed dummy data: {ex.Message}");
            }
        }

        public static async Task<bool> IsDatabaseEmptyAsync()
        {
            try
            {
                var db = await GetDatabaseAsync();
                var productCount = await db.Table<Product>().CountAsync();
                var invoiceCount = await db.Table<Invoice>().CountAsync();

                return (productCount + invoiceCount) == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DB ERROR] Error checking if DB is empty: {ex.Message}");
                return true; // Default to true so the app attempts to re-initialize if unreadable
            }
        }

        public static async Task ClearDatabaseAsync()
        {
            try
            {
                var db = await GetDatabaseAsync();

                // Wrapping in a transaction makes deleting thousands of rows nearly instantaneous and prevents partial wipes
                await db.RunInTransactionAsync(tran =>
                {
                    tran.DeleteAll<Invoice>();
                    tran.DeleteAll<InvoiceItem>();
                    tran.DeleteAll<Product>();
                    tran.DeleteAll<Customer>();
                    tran.DeleteAll<PaymentDetail>();
                });

                Debug.WriteLine("Garmetix DB: Cleared successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DB ERROR] Failed to clear database: {ex.Message}");
            }
        }

        public static async Task ResetDatabaseAsync()
        {
            try
            {
                await _initLock.WaitAsync();

                // 1. Properly close the connection first to release the file lock
                if (_database != null)
                {
                    await _database.CloseAsync();
                    _database = null;
                }
                _isInitialized = false;

                // 2. Allow the OS a brief moment to fully release the file handles
                await Task.Delay(100);

                // 3. Delete the file from the hard drive
                if (File.Exists(DatabasePath))
                {
                    File.Delete(DatabasePath);
                    Debug.WriteLine("Garmetix DB: Old file deleted.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CRITICAL DB ERROR] Failed to reset database: {ex.Message}");
            }
            finally
            {
                _initLock.Release();
            }

            // 4. Spin up a brand new, clean database
            await InitializeDatabaseAsync();
        }
    }
}