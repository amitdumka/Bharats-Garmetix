using Garmetix.HRM.PageModels;
using Garmetix.Base.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;

namespace Garmetix.HRM.Pages.Entry;

public class EntrySalaryPaySlipPage : ContentPage
{
    public EntrySalaryPaySlipPage(SalaryPaySlipFormModel fvm)
    {
        var efv = new EntryFormView { Title = "Salary Pay Slip New", ColumnCount = 2 };
        Content = new VerticalStackLayout
        {
            efv
        };
        fvm.InitFormViewModel();
        Title = "Salary Pay Slip[New]";
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
