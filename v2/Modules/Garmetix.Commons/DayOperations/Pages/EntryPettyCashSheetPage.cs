using Garmetix.Base.Views.Customs.Forms;
using Garmetix.Commons.DayOperations.PageModels;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.DayOperations.Pages
{
    public class EntryPettyCashSheetPage : ContentPage
    {
        public EntryPettyCashSheetPage(PettyCashSheetFormPageModel fvm)
        {
            var efv = new EntryFormView { Title = "Petty Cash Sheet New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Petty Cash Sheet [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Ledger", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}
