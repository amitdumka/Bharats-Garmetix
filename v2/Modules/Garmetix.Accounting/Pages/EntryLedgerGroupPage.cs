
using Garmetix.Accounting.PageModels.Parties;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.Pages
{
    public class EntryLedgerGroupPage : ContentPage
    {
        public EntryLedgerGroupPage(LedgerGroupFormModel fvm)
        {
            var efv = new EntryFormView { Title = "LedgerGroup New", ColumnCount = 1 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "LedgerGroup[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}