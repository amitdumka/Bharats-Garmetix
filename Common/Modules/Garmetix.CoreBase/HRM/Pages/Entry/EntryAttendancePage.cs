using Garmetix.CoreBase.HRM.PageModels;
using Garmetix.Core.Views.Customs.Forms;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.HRM.Pages.Entry;

public partial class EntryAttendancePage : ContentPage
{
    public EntryAttendancePage(AttendanceFormModel fvm)
    {
        var efv = new EntryFormView { Title = "New Attendance", ColumnCount = 2 };

        Content = new VerticalStackLayout { efv };

        fvm.InitFormViewModel();
        Title = "Attendance";
        efv.BindingContext = fvm;
        efv.DataForm.ItemsSourceProvider = fvm;
        efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
        efv.DataForm.RegisterEditor("Employee", DataFormEditorType.ComboBox);

        if (fvm != null)
        {
            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}