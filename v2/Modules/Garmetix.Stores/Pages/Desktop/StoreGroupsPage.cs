using Garmetix.CoreBase.Stores.PageModels;
using Garmetix.Base.Views.Customs.Listing;

namespace Garmetix.CoreBase.Stores.Pages.Desktop
{
    public class StoreGroupsPage : BaseListingPage
    {
        private readonly StoreGroupPageModel _viewModel;

        public StoreGroupsPage(StoreGroupPageModel vm)
        {
            try
            {
                Title = "Store Groups";
                //vm.AddUrl = $"Entry{nameof(StoreGroup)}Page";
                BindingContext = _viewModel = vm;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage("StoreGroupsPage Exp" + ex.Message);
            }
        }

        protected override async void OnAppearing()
        {
            //base.OnAppearing();

            try
            {
                //TODO: await _viewModel.HandleOnOnAppearing();
                //    if (_viewModel.Entities.Count == 0)
                //    {
                //        // The MVVM Toolkit's RelayCommand provides an ExecuteAsync method to await the task.
                //        await _viewModel.LoadInitialDataCommand.ExecuteAsync(null);
                //    }
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