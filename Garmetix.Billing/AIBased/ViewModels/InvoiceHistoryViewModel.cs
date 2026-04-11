using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers; // Ensure you have this for DatabaseHelper
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class InvoiceHistoryViewModel : ObservableObject
    {
        private List<Invoice> _allInvoices = new();
        private List<PaymentDetail> _allPayments = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private decimal totalSales;
        [ObservableProperty] private decimal totalReceived;
        [ObservableProperty] private decimal totalPending;

        [ObservableProperty] private ObservableCollection<Invoice> filteredInvoices = new();

        // --- SELECTION & MODAL STATE ---
        [ObservableProperty] private Invoice selectedInvoice;
        [ObservableProperty] private bool isDetailsModalVisible;

        // Data for the popup modal
        [ObservableProperty] private ObservableCollection<InvoiceItem> modalItems = new();
        [ObservableProperty] private Invoice modalInvoiceDetails;

        // --- FILTERS ---
        [ObservableProperty] private string searchText = string.Empty;
        public List<string> DateRanges { get; } = new() { "All Time", "Today", "Yesterday", "This Week", "This Month" };
        [ObservableProperty] private string selectedDateRange = "This Month";
        public List<string> PaymentModes { get; } = new() { "All Modes", "Cash", "UPI", "Card", "Bank Transfer" };
        [ObservableProperty] private string selectedPaymentMode = "All Modes";

        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedDateRangeChanged(string value) => ApplyFilters();
        partial void OnSelectedPaymentModeChanged(string value) => ApplyFilters();

        // Triggered the moment a user clicks a row in the DataGrid
        partial void OnSelectedInvoiceChanged(Invoice value)
        {
            if (value != null)
            {
                _ = HandleInvoiceSelectionAsync(value);
            }
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                var rawInvoices = await db.Table<Invoice>().ToListAsync();
                _allInvoices = rawInvoices.OrderByDescending(i => i.Date).ToList();
                _allPayments = await db.Table<PaymentDetail>().ToListAsync();

                MainThread.BeginInvokeOnMainThread(() => ApplyFilters());
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Load Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task HandleInvoiceSelectionAsync(Invoice invoice)
        {
            // 1. Show the native Action Sheet
            string action = await Application.Current.MainPage.DisplayActionSheet(
                $"Invoice {invoice.InvoiceNo}", "Cancel", "Delete Invoice", "View Details", "Edit Invoice");

            if (action == "View Details")
            {
                await OpenInvoiceDetailsModalAsync(invoice);
            }
            else if (action == "Delete Invoice")
            {
                await DeleteInvoiceAsync(invoice);
            }
            else if (action == "Edit Invoice")
            {
                // Navigate to the Edit Page and pass the unique Invoice ID securely
                await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={invoice.Id}");
            }

            // Deselect the row so it can be clicked again
            SelectedInvoice = null;
        }

        private async Task OpenInvoiceDetailsModalAsync(Invoice invoice)
        {
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();
                var items = await db.Table<InvoiceItem>().Where(i => i.InvoiceId == invoice.Id).ToListAsync();

                ModalInvoiceDetails = invoice;
                ModalItems = new ObservableCollection<InvoiceItem>(items);

                // Show the modal
                IsDetailsModalVisible = true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Details Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void CloseModal()
        {
            IsDetailsModalVisible = false;
            ModalItems.Clear();
            ModalInvoiceDetails = null;
        }

        private async Task DeleteInvoiceAsync(Invoice invoice)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Invoice",
                $"Are you sure you want to permanently delete {invoice.InvoiceNo}? This will remove all associated items and payments.",
                "Yes, Delete", "Cancel");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // Safely delete the invoice and its children in a transaction
                await db.RunInTransactionAsync(tran =>
                {
                    tran.Table<InvoiceItem>().Delete(i => i.InvoiceId == invoice.Id);
                    tran.Table<PaymentDetail>().Delete(p => p.InvoiceId == invoice.Id);
                    tran.Delete(invoice);
                });

                await Application.Current.MainPage.DisplayAlert("Deleted", "Invoice deleted successfully.", "OK");

                // Reload the table
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Delete Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddNewInvoiceAsync()
        {
            // Jumps directly to the Billing page using the Shell route defined in AppShell.xaml
            await Shell.Current.GoToAsync("//InvoiceEntryPage");
        }

        private void ApplyFilters()
        {
            var query = _allInvoices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();
                query = query.Where(i => (i.InvoiceNo != null && i.InvoiceNo.ToLower().Contains(search)) ||
                                         (i.CustomerName != null && i.CustomerName.ToLower().Contains(search)));
            }

            DateTime today = DateTime.Today;
            switch (SelectedDateRange)
            {
                case "Today": query = query.Where(i => i.Date.Date == today); break;
                case "Yesterday": query = query.Where(i => i.Date.Date == today.AddDays(-1)); break;
                case "This Week": query = query.Where(i => i.Date.Date >= today.AddDays(-(int)today.DayOfWeek)); break;
                case "This Month": query = query.Where(i => i.Date.Month == today.Month && i.Date.Year == today.Year); break;
            }

            if (SelectedPaymentMode != "All Modes")
            {
                var validIds = _allPayments.Where(p => p.Mode.Equals(SelectedPaymentMode, StringComparison.OrdinalIgnoreCase)).Select(p => p.InvoiceId).ToHashSet();
                query = query.Where(i => validIds.Contains(i.Id));
            }

            var finalResults = query.ToList();
            FilteredInvoices = new ObservableCollection<Invoice>(finalResults);

            TotalSales = finalResults.Sum(i => i.GrandTotal);
            TotalReceived = finalResults.Sum(i => i.PaidAmount);
            TotalPending = finalResults.Sum(i => i.BalanceAmount);
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}