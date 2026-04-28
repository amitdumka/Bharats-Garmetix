using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Accounting.Pages
{
    public class BankTransactionPage : BaseListingPage
    {
        private readonly BankTransactionPageModel _viewModel;
         
        public BankTransactionPage(BankTransactionPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
    public class ChequeLogPage : BaseListingPage
    {
        private readonly ChequeLogPageModel _viewModel;
         
        public ChequeLogPage(ChequeLogPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
}
