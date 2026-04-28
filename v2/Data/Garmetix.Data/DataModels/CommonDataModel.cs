

using Garmetix.Core.VM;
using Garmetix.Databases;

namespace Garmetix.Core.DataModels
{
    public class CommonDataModel
    {
        public static List<ComboBoxItemVM> GetEmployeeList(DatabaseContext db, Guid CompanyId) => db.Employees.Where(c => c.CompanyId == CompanyId).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.FullName }).ToList();

        public static List<ComboBoxItemVM> GetEmployeeList(DatabaseContext db) => db.Employees.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.FullName }).ToList();

        public static List<ComboBoxItemVM> GetStoreList(DatabaseContext db) => db.Stores.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetCompanyList(DatabaseContext db) => db.Companies.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetGroupList(DatabaseContext db) => db.StoreGroups.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetLedgerGroupList(DatabaseContext db) => db.LedgerGroups.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetLedgerList(DatabaseContext db) => db.Ledgers.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetTransactionList(DatabaseContext db) => db.Transactions.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetSalesmanList(DatabaseContext db, Guid StoreId) => db.Salesmen.Where(c => c.StoreId == StoreId && !c.Deleted).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetSalesmanList(DatabaseContext db) => db.Salesmen.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetBankAccountList(DatabaseContext db, Guid CompanyId) => db.BankAccounts.Where(c => c.CompanyId == CompanyId).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.AccountHolderName + " (" + c.AccountNumber + ")" }).ToList();

        public static List<ComboBoxItemVM> GetBankAccountList(DatabaseContext db) => db.BankAccounts.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.AccountHolderName + " (" + c.AccountNumber + ")" }).ToList();

        public static List<ComboBoxItemVM> GetBankList(DatabaseContext db) => db.Banks.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetVendorList(DatabaseContext db) => db.Vendors.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetPartiesList(DatabaseContext db) => db.Parties.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetInvoiceNumberList(DatabaseContext db) => db.Invoices.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        public static List<ComboBoxItemVM> GetPurchaseInvoiceNumberList(DatabaseContext db) => db.PurchaseInvoices.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        public static List<ComboBoxItemVM> GetDueInvoiceNumberList(DatabaseContext db) => db.CustomerDues.Where(c => !c.Paid).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        internal static List<ComboBoxItemVM> GetProductList(DatabaseContext db) => db.Products.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        internal static List<ComboBoxItemVM> GetProductCategoryList(DatabaseContext db) => db.ProductCategories.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        internal static List<ComboBoxItemVM> GetProductSubCategoryList(DatabaseContext db) => db.ProductSubCategories.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        internal static List<ComboBoxItemVM> GetTaxList(DatabaseContext db) => db.Taxes.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();
    }
}