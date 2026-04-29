using Garmetix.Base.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Entry;

public class EntryTimeSheetPage : ContentPage
{
    public EntryTimeSheetPage(TimeSheetFormModel fvm)
    {
        var efv = new EntryFormView { Title = "Time Sheet New", ColumnCount = 2 };

        Content = new VerticalStackLayout
        {
            efv
        };

        fvm.InitFormViewModel();
        Title = "Time Sheet[New]";
        efv.BindingContext = fvm;
        efv.DataForm.ItemsSourceProvider = fvm;
        efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Employee", DataFormEditorType.ComboBox);

        efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
    }
}
