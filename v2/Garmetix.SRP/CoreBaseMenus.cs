 
using Garmetix.Accounting.Pages; 
using Garmetix.Base.Shells;
using Garmetix.Billing.Pages;
using Garmetix.Core.Sessions;
using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.Stores.Pages.Desktop;
using Garmetix.CoreBase.Stores.Pages.Mobile;

// Use C# using aliases to separate the mobile and desktop pages
using HrmPages = Garmetix.HRM.Pages.Desktop;
using MobileHrmPages = Garmetix.HRM.Pages.Mobile;

namespace Garmetix.SRP
{
    public partial class BillingMenu: BaseFlyoutMenu
    {
        public BillingMenu():base("Billing")
        {
            AddPageTab("Sale Invoice", "rain_icon.png", "SaleInvoice", typeof(InvoiceHistoryPage));
            //AddPageTab("Sale's Return Invoice", "rain_icon.png", "SalesReturnInvoice", typeof(InvoiceReturnHistoryPage));
            //AddPageTab("Invoice Payments", "rain_icon.png", "InvoicePayments", typeof(InvoicePaymentHistoryPage));
            //AddPageTab("Card Payments", "rain_icon.png", "CardPayments", typeof(CardPaymentHistoryPage));
            //AddPageTab("Purchase Invoice", "rain_icon.png", "PurchaseInvoice", typeof(PurchaseInvoiceHistoryPage));
            //AddPageTab("Purchase Return Invoice", "rain_icon.png", "PurchaseReturnInvoice", typeof(PurchaseReturnInvoiceHistoryPage));
            //AddPageTab("Purchase Invoice Payments", "rain_icon.png", "PurchaseInvoicePayments", typeof(PurchaseInvoicePaymentHistoryPage));
            //AddPageTab("Purchase Orders", "rain_icon.png", "PurchaseOrders", typeof(PurchaseOrderHistoryPage));
            
        }
    }
    public partial class CompanyMenu : BaseFlyoutMenu
    {
        public CompanyMenu() : base("Stores")
        {
            // --- Platform Specific Tabs ---
            AddPlatformSpecificPageTab("Store", "rain_icon.png", "Stores",
                mobilePageType: typeof(StorePage),
                desktopPageType: typeof(StoresPage));

            AddPlatformSpecificPageTab("Store Group", "rain_icon.png", "Group",
                mobilePageType: typeof(StoreGroupPage),
                desktopPageType: typeof(StoreGroupsPage));

            AddPlatformSpecificPageTab("Company", "rain_icon.png", "Client",
                mobilePageType: typeof(CompanyPage),
                desktopPageType: typeof(CompaniesPage));

            // --- Standard Tabs ---
             AddPageTab("Day Begin", "rain_icon.png", "daybegib", typeof(DayBeginEntyPage));
             AddPageTab("Day Closing", "rain_icon.png", "dayend", typeof(DayEndEntryPage));
             AddPageTab("Petty Cash Sheet", "rain_icon.png", "pettycashsheet", typeof(PettyCashSheetEntryPage));
        }
    }

    public class AccountingMenu : BaseFlyoutMenu
    {
        public AccountingMenu() : base("Vouchers") // The main menu title
        {
            // Just call the helper method for each page!
            AddPageTab("Voucher", "rain_icon.png", "Voucher", typeof(VoucherPage));
            AddPageTab("Cash Voucher", "rain_icon.png", "CashVoucher", typeof(CashVoucherPage));
        }
    }

    public class AccountsMenu : BaseFlyoutMenu
    {
        public AccountsMenu() : base("Accounts") // The main menu title
        {
            // Just call the helper method for each page!

            AddPageTab("Customer Dues", "rain_icon.png", "CustomerDues", typeof(CustomerDuePage));
            AddPageTab("Due Recovery", "rain_icon.png", "DueRecovery", typeof(DueRecoveryPage));
            AddPageTab("Transaction", "rain_icon.png", "Transaction", typeof(TransactionPage));
            AddPageTab("Petty Cash", "rain_icon.png", "PettyCash", typeof(PettyCashSheetPage));
            AddPageTab("Cash Details", "rain_icon.png", "CashBook", typeof(CashDetailPage));
        }
    }

    public class  BankingMenu: BaseFlyoutMenu//ToDO: need to check with generate code for this menu, as it is not yet implemented in the codebase.
    {
        public BankingMenu() : base("Banking")
        {
            // Standard Tabs
            AddPageTab("Bank Accounts", "rain_icon.png", "BankAccounts", typeof(BankAccountPage));
            AddPageTab("Banks", "rain_icon.png", "Banks", typeof(BankPage));

            // Secure Tab: Only visible if SessionService.IsAdminUser is true
            AddPageTab("Account Details", "rain_icon.png", "BankAccountDetails", typeof(BankAccountDetailPage), SessionService.IsAdminUser);

            // Remaining Tabs
            AddPageTab("Account List", "rain_icon.png", "AccountList", typeof(BankAccountListPage));
            AddPageTab("Vendor Accounts", "rain_icon.png", "VendorAccounts", typeof(VendorBankAccountPage));

            AddPageTab("Transactions", "rain_icon.png", "BankTransactions", typeof(BankTransactionPage));
            AddPageTab("Deposit/Withdrawal", "rain_icon.png", "BankCashTransactions", typeof(BankCashTransactionPage));
            AddPageTab("Cheque Logs", "rain_icon.png", "ChequeLogs", typeof(ChequeLogPage));
        }

    }


   
    public class LedgerMenu : BaseFlyoutMenu
    {
        public LedgerMenu() : base("Ledger")
        {
            // Just call the helper method for each page!
            AddPageTab("Party", "rain_icon.png", "Party", typeof(PartyPage));
            AddPageTab("Ledger", "rain_icon.png", "Ledger", typeof(LedgerPage));
            AddPageTab("Ledger Group", "rain_icon.png", "LedgerGroup", typeof(LedgerGroupPage));

        }
    }

    public class HRMMenu : BaseFlyoutMenu
    {
        public HRMMenu() : base("HRM")
        {
            // 1. Attendance (Platform Specific)
            AddPlatformSpecificPageTab("Attendance", "rain_icon.png", "Attendance",
                mobilePageType: typeof(MobileHrmPages.AttendancePage),

 
                desktopPageType: typeof(HrmPages.AttendancePage));

            // 2. Time Sheet (Platform Specific & Hidden)
            AddPlatformSpecificPageTab("Time Sheet", "rain_icon.png", "TimeSheets",
                mobilePageType: typeof(MobileHrmPages.TimeSheetPage),
                desktopPageType: typeof(HrmPages.TimeSheetPage),
                isVisible: false);

            // 3. Salary (Platform Specific)
            AddPlatformSpecificPageTab("Salary", "rain_icon.png", "Salarys",
                mobilePageType: typeof(MobileHrmPages.SalaryPaymentPage),
                desktopPageType: typeof(HrmPages.SalaryPaymentPage));

            // 4. Employees (Standard)
            AddPageTab("Employees", "rain_icon.png", "Employees",
                typeof(HrmPages.EmployeesPage));

            // 5. PaySlip (Standard & Hidden)
            AddPageTab("PaySlip", "rain_icon.png", "SalaryPaySlipPage",
                typeof(HrmPages.SalaryPaySlipPage),
                isVisible: false);

            // 6. Salary Structures (Standard & Hidden)
            AddPageTab("Salary Structures", "rain_icon.png", "SalaryStructurePage",
                typeof(HrmPages.SalaryStructurePage),
                isVisible: false);

            // 7. Monthly Attendance (Platform Specific)
            AddPlatformSpecificPageTab("Monthly Attendance", "rain_icon.png", "MonthlyAttendancePage",
                mobilePageType: typeof(MobileHrmPages.MonthlyAttendancePage),
                desktopPageType: typeof(HrmPages.MonthlyAttendancePage));
        }
    }
}