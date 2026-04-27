using Bharat.ToolKits.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.Enums;

namespace Garmetix.Core.PageModels
{
    [ObservableRecipient]
    public partial class PageModelBase : ObservableValidator
    {
        [ObservableProperty]
        protected AppOperation _appOperations = StorageOps.GetPref("AppOperation", AppOperation.Store);

        [ObservableProperty]
        protected string _icon;

        [ObservableProperty]
        protected UserType _role = UserType.Guest;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        protected bool _isBusy;

        public bool IsNotBusy => !IsBusy;

        protected bool _isNavigatedTo;
        protected static DatabaseContext _db => DatabaseService.Instance.LocalDB;

        [RelayCommand]
        private void NavigatedTo() =>
            _isNavigatedTo = true;

        [RelayCommand]
        private void NavigatedFrom() =>
            _isNavigatedTo = false;

        [RelayCommand]
        public virtual Task Appearing()
        {
            return Task.CompletedTask;
        }

        ////Edit Delete
        //private bool CanExecute(object arg)
        //{
        //    return arg != null;
        //}

        ///// <summary>
        ///// Logic for editing an item.
        ///// </summary>
        //private async void OnEdit(object itemToEdit)
        //{
        //    if (itemToEdit is OrderInfo order)
        //    {
        //        // In a real app, you would navigate to an edit page.
        //        // For this example, we'll just show an alert.
        //        await Application.Current.MainPage.DisplayAlert("Edit", $"Editing Order: {order.OrderID}", "OK");

        //        // Example: Navigation.PushAsync(new EditPage(order));
        //    }
        //}

        ///// <summary>
        ///// Logic for deleting an item.
        ///// </summary>
        //private async void OnDelete(object itemToDelete)
        //{
        //    if (itemToDelete is OrderInfo order)
        //    {
        //        bool confirmed = await Application.Current.MainPage.DisplayAlert(
        //            "Confirm Delete",
        //            $"Are you sure you want to delete Order: {order.OrderID}?",
        //            "Yes", "No");

        //        if (confirmed)
        //        {
        //            Entities.Remove(order);
        //            await Application.Current.MainPage.DisplayAlert("Success", "Item Deleted", "OK");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Refreshes the data in the grid.
        ///// </summary>
        //private async Task RefreshData()
        //{
        //    // Here you would re-fetch your data from a service or database
        //    await Task.Delay(1000); // Simulate network delay
        //}
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        ///// <summary>
        ///// Navigates to the Edit Page, passing the selected item as an object.
        ///// </summary>
        //private async void OnEdit(object itemToEdit)
        //{
        //    if (itemToEdit is OrderInfo order)
        //    {
        //        // Use Shell navigation to go to the EditPage.
        //        // We pass the entire object using a navigation dictionary.
        //        // This is preferred for complex objects.
        //        var navigationParameters = new Dictionary<string, object>
        //    {
        //        { "OrderToEdit", order }
        //    };
        //        await Shell.Current.GoToAsync("///EditPage", true, navigationParameters);
        //    }
        //}

        ///// <summary>
        ///// Navigates to the Delete Page, passing the ID of the selected item.
        ///// </summary>
        //private async void OnDelete(object itemToDelete)
        //{
        //    if (itemToDelete is OrderInfo order)
        //    {
        //        // Use Shell navigation to go to the DeletePage.
        //        // We pass the ID as a query parameter in the route.
        //        await Shell.Current.GoToAsync($"///DeletePage?id={order.Id}");
        //    }
        //}

        ////End of Edit Delete

        [RelayCommand]
        public virtual void Edit(Object itemToEdit)
        {
        }

        [RelayCommand]
        public virtual void Delete(Object itemToDelete)
        {
        }
    }
}