

using Garmetix.Base.Views.Customs.Listing;

namespace Garmetix.Accounting.Pages
{
    public class VoucherPage : BaseListingPage
    {
        private readonly VoucherPageModel _viewModel;
        public VoucherPage(VoucherPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";

            BindingContext = _viewModel = vm;

        }
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
    }
}
