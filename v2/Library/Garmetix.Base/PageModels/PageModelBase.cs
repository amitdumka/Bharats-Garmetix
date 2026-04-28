/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * PageModelBase.cs
 *
 *PageModelBase is the base class for all page models in the Garmetix application. It provides common properties and commands that are used by all page models, such as handling navigation events, managing busy states, and defining user roles. By inheriting from this base class, individual page models can focus on their specific functionality while still maintaining a consistent structure and behavior across the application.
 *
 *This file is part of Garmetix, a comprehensive inventory management system designed for garment businesses. Garmetix provides tools for managing inventory, tracking orders, and analyzing sales data to help businesses optimize their operations and increase profitability. The PageModelBase class serves as the foundational base class for all page models in the application, providing common properties and commands that are used across different pages. This includes handling navigation events, managing busy states, and defining user roles, ensuring a consistent structure and behavior throughout the application.
 */

using Bharat.ToolKits.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.Enums;
using Garmetix.Databases;
using Garmetix.Databases.Services; 

namespace Garmetix.Base.PageModels
{
    /// <summary>
    /// PageModelBase is the base class for all page models in the application. It provides common properties and commands that are used by all page models. This includes properties for app operations, user role, busy state, and navigation state, as well as commands for handling navigation events and editing/deleting items. By inheriting from this base class, individual page models can focus on their specific functionality while still maintaining a consistent structure and behavior across the application.
    /// </summary>
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