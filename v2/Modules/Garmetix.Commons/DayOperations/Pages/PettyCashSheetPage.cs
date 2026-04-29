using Garmetix.Base.Views.Customs.Forms;
using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Commons.DayOperations.PageModels;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.DayOperations.Pages
{
   public partial class PettyCashSheetPage:BaseListingPage    
    {
        //private readonly PettyCashSheetPageModel _viewModel;
        
        public PettyCashSheetPage(PettyCashSheetPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = vm;
        }
    }
    public partial class CashDetailPage : BaseListingPage
    {
        public CashDetailPage(CashDetailPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = vm;
        }
    }
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
    public class EntryCashDetailPage : ContentPage
    {
        public EntryCashDetailPage(CashDetailFormPageModel fvm)
        {
            var efv = new EntryFormView { Title = "Cash Detail New", ColumnCount = 2 };

            Content = new VerticalStackLayout
            {
                efv
            };

            fvm.InitFormViewModel();
            Title = "Cash Detail [New]";
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
