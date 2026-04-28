using Garmetix.Base.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.Pages
{
    public class EntryBankTransactionPage : ContentPage
    {
        public EntryBankTransactionPage(BankTransactionFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Bank Transaction New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Bank Transaction[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("BankAccount", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}