using Garmetix.CoreBase.DayOperations.Pages;
using Garmetix.CoreBase.Stores.Pages.Desktop;
using Garmetix.CoreBase.Stores.Pages.Mobile;

namespace Garmetix.CoreBase.View
{

    public class Accounting_Menu : BaseFlyoutMenu
    {
        public Accounting_Menu() : base("Vouchers") // The main menu title
        {
            // Just call the helper method for each page!
            AddPageTab("Voucher", "rain_icon.png", "Voucher", typeof(VoucherPage));
            AddPageTab("Cash Voucher", "rain_icon.png", "CashVoucher", typeof(CashVoucherPage));
        }
    }

    public class Accounts_Menu : BaseFlyoutMenu
    {
        public Accounts_Menu() : base("Accounts") // The main menu title
        {
            // Just call the helper method for each page!

            AddPageTab("Customer Dues", "rain_icon.png", "CustomerDues", typeof(CustomerDuePage));
            AddPageTab("Due Recovery", "rain_icon.png", "DueRecovery", typeof(DueRecoveryPage));
            AddPageTab("Transaction", "rain_icon.png", "Transaction", typeof(TransactionPage));
            AddPageTab("Petty Cash", "rain_icon.png", "PettyCash", typeof(PettyCashSheetPage));
            AddPageTab("Cash Details", "rain_icon.png", "CashBook", typeof(CashDetailPage));
        }
    }

    public class  Banking_Menu: BaseFlyoutMenu//ToDO: need to check with generate code for this menu, as it is not yet implemented in the codebase.
    {
        public Banking_Menu() : base("Banking")
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
            AddPageTab("Cheque Logs", "rain_icon.png", "ChequeLogs", typeof(ChequeLogPage));
        }

    }


    public class Company_Menu : BaseFlyoutMenu
    {
        public Company_Menu() : base("Stores")
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
            AddPageTab("Petty Cash Sheet", "rain_icon.png", "cashsheet", typeof(PettyCashSheetEntryPage));
        }
    }
}