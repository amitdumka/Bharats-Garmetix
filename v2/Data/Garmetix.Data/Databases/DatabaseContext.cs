using Bharat.ToolKits.Helpers;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Authentication;
using Garmetix.Core.Models.HRM;
using Garmetix.Core.Models.Inventory;
using Garmetix.Core.Models.Stores;
using Garmetix.Models.DayOperations;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Databases
{
    /// <summary>
    ///  Local Database in App, it can be used for caching of data or for offline use.
    /// </summary>
    public partial class DatabaseContext : DbContext//, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable, IResettableService, IDisposable, IAsyncDisposable
    {
        private static DatabaseContext? _instance;
        public static DatabaseContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseContext(Constants.CompanyDatabaseFileName);
                }
                return _instance;
            }
        }
        public string DatabasePath { get; set; } = Constants.DatabasePath;
        public void Reconfigure(string databasePath)
        {
            try
            {
                Constants.CompanyDatabaseFileName = databasePath;
                DatabasePath = Path.Combine(FileSystem.Current.AppDataDirectory, databasePath);
                Database.GetDbConnection().ConnectionString = $"Filename={DatabasePath}";
                StorageOps.SetPref("CompanyDatabaseFileName", DatabasePath);
                var x = Database.EnsureCreated();
                Console.WriteLine(" Database: changed and create " + x);
                _instance = this;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// This is a constructor that initializes the database context with a specific filename.
        /// </summary>
        /// <param name="filename"></param>
        public DatabaseContext(string filename)
        {
            try
            {
                DatabasePath = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
                Console.WriteLine("DBPath:" + DatabasePath);
                SQLitePCL.Batteries_V2.Init();
                var x = Database.EnsureCreated();
                if (x)
                {
                    Console.WriteLine("Database created successfully.");
                }
                else
                {
                    Console.WriteLine("Database already exists.");
                }
                _instance = this;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// This is a constructor that initializes the database context. for dummy database. initially it will create a database with default name.
        /// </summary>
        public DatabaseContext()
        {
            try
            {
                DatabasePath = Constants.CompanyDatabasePath;
                Console.WriteLine("DBPath:" + DatabasePath);
                SQLitePCL.Batteries_V2.Init();
                var x = Database.EnsureCreated();
                if (x)
                {
                    Console.WriteLine("Database created successfully.");
                }
                else
                {
                    Console.WriteLine("Database already exists.");
                }
                _instance = this;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class using the specified options.
        /// </summary>
        /// <remarks>This constructor sets up the database context, initializes the SQLite library, and
        /// ensures that the database is created if it does not already exist. The database path is set to a predefined
        /// constant value.</remarks>
        /// <param name="options">The options to configure the <see cref="DatabaseContext"/> instance, such as the database connection string
        /// and other settings.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        {
            try
            {
                if (Constants.CompanyDatabaseFileName.Contains("dummy"))
                {
                    Constants.CompanyDatabaseFileName = Preferences.Get("CompanyDatabaseFileName", "dummydb.db3");
                    DatabasePath = Constants.CompanyDatabasePath;
                    SQLitePCL.Batteries_V2.Init();
                    var x = Database.EnsureCreated();
                }
                else
                {
                    DatabasePath = Constants.CompanyDatabasePath;
                    SQLitePCL.Batteries_V2.Init();
                    var x = Database.EnsureCreated();

                }
                _instance = this;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Configures the database connection for the <see cref="DatabaseContext"/> instance.
        ///
        /// </summary>
        /// <param name="optionsBuilder">The options builder used to configure the database connection.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlite($"Filename={DatabasePath}");
                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public static void ApplyMigrations(DatabaseContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee and SalaryStructure
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.SalaryStructures)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId);

            // Employee and Attendance
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Attendances)
                .WithOne(a => a.Employee)
                .HasForeignKey(a => a.EmployeeId);

            // Employee and SalaryPayment
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.SalaryPayments)
                .WithOne(sp => sp.Employee)
                .HasForeignKey(sp => sp.EmployeeId);
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<StoreGroup> StoreGroups { get; set; }
        public DbSet<Store> Stores { get; set; }

        //TODO: create if def option to create tables
        public DbSet<LedgerGroup> LedgerGroups { get; set; }

        public DbSet<Party> Parties { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<CashVoucher> CashVouchers { get; set; }
        public DbSet<DueRecovery> DueRecovery { get; set; }
        public DbSet<CustomerDue> CustomerDues { get; set; }

        //Banking

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BankAccountDetail> BankAccountDetails { get; set; }
        public DbSet<VendorBankAccount> VendorBankAccounts { get; set; }
        public DbSet<BankAccountList> BankAccountLists { get; set; }
        //public DbSet<BankTransaction> BankTransactions { get; set; }
        //public DbSet<ChequeLog> ChequeLogs { get; set; }

        //HRM
        public DbSet<Employee> Employees { get; set; }

        public DbSet<EmployeeDetail> EmployeeDetails { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<MonthlyAttendance> MonthlyAttendances { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<SalaryPayment> SalaryPayments { get; set; }
        public DbSet<SalaryPaySlip> SalaryPaySlips { get; set; }
        public DbSet<SalaryStructure> SalaryStructures { get; set; }

        //Inventory
        public DbSet<Product> Products { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductSubCategory> ProductSubCategories { get; set; }

        public DbSet<Tax> Taxes { get; set; }

        //Invoicing
        public DbSet<Salesman> Salesmen { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoicePayment> InvoicePayments { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<CardPayment> CardPayments { get; set; }
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
        public DbSet<VendorPayment> VendorPayments { get; set; }

        //Auth
        public DbSet<AppUser> AppUsers { get; set; }

        //Day Operation 
        public DbSet<DayBegin> DayBegins { get; set; }
        public DbSet<DayEnd> DayEnds { get; set; }
        public DbSet<PettyCashSheet> PettyCashSheets { get; set; } 
        public DbSet<CashDetail> CashDetails { get; set; }
    }
}