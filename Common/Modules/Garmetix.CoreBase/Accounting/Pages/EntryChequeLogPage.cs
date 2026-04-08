using Garmetix.Core.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.Pages
{
    public class EntryChequeLogPage : ContentPage
    {
        public EntryChequeLogPage(ChequeLogFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Cheque Log New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Cheque Log[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("BankAccount", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}