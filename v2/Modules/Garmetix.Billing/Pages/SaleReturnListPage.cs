using Garmetix.Base.Views.Customs.Listing;
using Garmetix.Billing.PageModels;

namespace Garmetix.Billing.Pages
{
    public partial class SaleReturnListPage: BaseListingPage
    {
        private SaleReturnPageModel _viewModel;
        public SaleReturnListPage(SaleReturnPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
}
