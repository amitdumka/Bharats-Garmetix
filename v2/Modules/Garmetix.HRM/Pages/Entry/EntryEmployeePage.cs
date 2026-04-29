using Garmetix.HRM.PageModels;
using Garmetix.Base.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;

namespace Garmetix.HRM.Pages.Entry;

public class EntryEmployeePage : ContentPage
{

    public EntryEmployeePage(EmployeeFormModel fvm)
    {
        var efv = new EntryFormView { Title = "New Employee", ColumnCount = 2 };

        Content = new VerticalStackLayout
            {
                efv
            };

        fvm.InitFormViewModel();
        Title = "Employee";
        efv.BindingContext = fvm;
        efv.DataForm.ItemsSourceProvider = fvm;
        efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
        efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
    }
}
