using Garmetix.Core.Views.Customs.Listing;
using Bharat.ToolKits.Extensions;

namespace Garmetix.CoreBase.Accounting.Pages
{
    public class BankAccountDetailPage : BaseListingPage
    {
        private readonly BankAccountDetailPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}

        public BankAccountDetailPage(BankAccountDetailPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
    }
}
