using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.Pages
{
    public class EntryVoucherPage : ContentPage
    {
        public EntryVoucherPage(VoucherFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Voucher New", ColumnCount = 2 };

            Content = new VerticalStackLayout
        {
            efv
        };

            fvm.InitFormViewModel();
            Title = "Voucher[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Bank", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Ledger", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Employee", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("AccountNumber", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}