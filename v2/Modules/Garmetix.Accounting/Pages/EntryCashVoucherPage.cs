
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.Pages
{
    public class EntryCashVoucherPage : ContentPage
    {
        public EntryCashVoucherPage(CashVoucherFormModel fvm)
        {
            var efv = new EntryFormView { Title = "CashVoucher New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "CashVoucher[New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Transaction", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Employee", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Ledger", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}