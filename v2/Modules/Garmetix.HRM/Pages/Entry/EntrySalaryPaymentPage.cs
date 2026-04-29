using Garmetix.HRM.PageModels; 
using Syncfusion.Maui.DataForm;
using Garmetix.Base.Views.Customs.Forms;

namespace Garmetix.HRM.Pages.Entry;

public class EntrySalaryPaymentPage : ContentPage
{
    public EntrySalaryPaymentPage(SalaryPaymentFormModel fvm)
    {
        var efv = new EntryFormView { Title = "New Salary Payment", ColumnCount = 2 };

        Content = new VerticalStackLayout
        {
            efv
        };

        fvm.InitFormViewModel();
        Title = "Salary Payment";
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
