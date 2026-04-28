using Bharat.ToolKits.Extensions;
using Garmetix.Accounting.PageModels.Vouchers;

namespace Garmetix.Accounting.Pages
{
    public class CashVoucherPage : BaseListingPage
    {
        private readonly CashVoucherPageModel _viewModel;
        public CashVoucherPage(CashVoucherPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
    }
}
