using Bharat.ToolKits.Extensions;
using Garmetix.Accounting.PageModels.Parties;

namespace Garmetix.Accounting.Pages
{
    public class LedgerPage : BaseListingPage
    {
        private readonly LedgerPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public LedgerPage(LedgerPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
    public class DueRecoveryPage : BaseListingPage
    {

        private readonly DueRecoveryPageModel _viewModel;
        public DueRecoveryPage(DueRecoveryPageModel vm)
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
    public class CustomerDuePage : BaseListingPage
    {
        private readonly CustomerDuePageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();
        //}
        public CustomerDuePage(CustomerDuePageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
    }

    public class PartyPage : BaseListingPage
    {
        private readonly PartyPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public PartyPage(PartyPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
    public class LedgerGroupPage : BaseListingPage
    {
        private readonly LedgerGroupPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public LedgerGroupPage(LedgerGroupPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
    }

    public class TransactionPage : BaseListingPage
    {
        private readonly TransactionPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public TransactionPage(TransactionPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }
    public class BankPage : BaseListingPage
    {
        private readonly BankPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public BankPage(BankPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            BindingContext = _viewModel = vm;
        }
    }

    public class BankAccountPage : BaseListingPage
    {
        private readonly BankAccountPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public BankAccountPage(BankAccountPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
    }
    public class VendorBankAccountPage : BaseListingPage
    {
        private readonly VendorBankAccountPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        //}
        public VendorBankAccountPage(VendorBankAccountPageModel vm)
        {
            // Set the Title to the class name without the "Page" suffix
            var className = GetType().Name;
            Title = className.EndsWith("Page") ? className[..^4] : className;
            vm.AddUrl = $"Entry{Title}Page";
            Title = Title.SplitPascalCase_Simple();
            BindingContext = _viewModel = vm;
        }
    }
    public class BankAccountListPage : BaseListingPage
    {
        private readonly BankAccountListPageModel _viewModel;
        //protected override async void OnAppearing()
        //{
        //    //base.OnAppearing();
        //    //TODO: await _viewModel.HandleOnOnAppearing();

        ////}
        public BankAccountListPage(BankAccountListPageModel vm)
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
