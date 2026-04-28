using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Core.Views.Customs.Listing;

namespace Garmetix.CoreBase.Stores.Pages.Desktop
{

    public class CompaniesPage : BaseListingPage
    {
        private readonly CompanyPageModel _viewModel;

        public CompaniesPage(CompanyPageModel vm)
        {
            try
            {
                Title = "Companies";
                //vm.AddUrl = $"Entry{nameof(Company)}Page";
                BindingContext = _viewModel = vm;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage("CompaniesPage Exp" + ex.Message);
            }
        }

        protected override async void OnAppearing()
        {
            //base.OnAppearing();

            try
            {
                if (_viewModel.Entities.Count == 0)
                {
                    // The MVVM Toolkit's RelayCommand provides an ExecuteAsync method to await the task.
                    await _viewModel.LoadInitialDataCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.ConfigureScope(scope => scope.SetTag("Page", "StoresPage"));
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage("Salesanpage Exp" + ex.Message);

                await DisplayAlert("Error", "Failed to load Store data. Please try again later.", "OK");
            }
        }
    }
}