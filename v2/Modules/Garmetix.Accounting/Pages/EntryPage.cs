using Garmetix.Accounting.PageModels.Parties;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.Pages
{

    public partial class EntryDueRecoveryPage : ContentPage
    {
        public EntryDueRecoveryPage(DueRecoveryFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Due Recovery New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Due Recovery [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("DueInvoiceNumber", DataFormEditorType.ComboBox);
          

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryCustomerDuePage : ContentPage
    {
        public EntryCustomerDuePage(CustomerDueFormModel fvm)
        {
            var efv = new EntryFormView { Title = "CustomerDue New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "CustomerDue[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            // efv.DataForm.RegisterEditor("InvoiceNumber", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryLedgerPage : ContentPage
    {
        public EntryLedgerPage(LedgerFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Ledger New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Ledger[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("LedgerGroup", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryTransactionPage : ContentPage
    {
        public EntryTransactionPage(TransactionFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Transcation New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Transcation[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryBankAccountPage : ContentPage
    {
        public EntryBankAccountPage(BankAccountFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Bank Account New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Bank Account [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryBankAccountDetailPage : ContentPage
    {
        public EntryBankAccountDetailPage(BankAccountDetailFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Account Details New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Account Details [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("BankAccount", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryVendorBankAccountPage : ContentPage
    {
        public EntryVendorBankAccountPage(VendorBankAccountFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Vendor Account New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Vendor Account Details [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Vendor", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public partial class EntryBankAccountListPage : ContentPage
    {
        public EntryBankAccountListPage(BankAccountListFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Account List New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Account List [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            //  efv.DataForm.RegisterEditor("BankAccount", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}