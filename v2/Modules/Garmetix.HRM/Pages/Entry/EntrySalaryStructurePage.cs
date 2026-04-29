using Garmetix.Base.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;
using Garmetix.HRM.PageModels;

namespace Garmetix.HRM.Pages.Entry;

public class EntrySalaryStructurePage : ContentPage
{
    public EntrySalaryStructurePage(SalaryStructureFormModel fvm)
    {
        var efv = new EntryFormView { Title = "Salary Structure New", ColumnCount = 2 };
        Content = new VerticalStackLayout
        {
            efv
        };
        fvm.InitFormViewModel();
        Title = "Salary Structure[New]";
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
