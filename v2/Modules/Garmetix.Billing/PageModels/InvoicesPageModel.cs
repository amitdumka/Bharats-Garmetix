using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels
{
    public partial class InvoicesPageModel : ObservableObject
    {
        //Database Context
        private DatabaseContext _localdb = DatabaseService.Instance.LocalDB;

        public DatabaseContext GetContext()
        { return _localdb; }

        // Invoice Service
        private InvoiceService _invoiceService;

        //Invoice Details
        private List<Invoice> _allInvoices = new();

        private List<InvoicePayment> _allPayments = new();
        private List<CardPayment> _allCards = new();

        //Operation
        [ObservableProperty] private bool isBusy;

        //Sale Information and Payments
        [ObservableProperty] private decimal totalSales;

        [ObservableProperty] private decimal totalReceived;
        [ObservableProperty] private decimal totalPending;

        //Filtered Invoice
        [ObservableProperty] private ObservableCollection<Invoice> filteredInvoices = new();

        // --- SELECTION & MODAL STATE ---
        [ObservableProperty] private Invoice? selectedInvoice;

        [ObservableProperty] private bool isDetailsModalVisible;

        // Data for the popup modal
        [ObservableProperty] private ObservableCollection<InvoiceItem> modalItems = new();

        [ObservableProperty] private Invoice? modalInvoiceDetails;

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
        partial void OnSelectedInvoiceChanged(Invoice? value)
        {
            if (value != null)
            {
                _ = HandleInvoiceSelectionAsync(value);
            }
        }

        public InvoicesPageModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        //Loading Inital Data
        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                //var rawInvoices = await GetContext().Invoices.ToListAsync();
                //_allInvoices = rawInvoices.OrderByDescending(i => i.OnDate).ToList();
                //TODO: instead in view model it should be there in invoice service

                _allInvoices = await _invoiceService.GetInvoicesAsync();
                _allPayments = await _invoiceService.GetPaymentsAsync();
                _allCards = await _invoiceService.GetCardPaymentsAsync();

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

        /// <summary>
        /// Apply filters
        /// </summary>
        private async void ApplyFilters()
        {
            //var query = _allInvoices.AsEnumerable();
            var query = (await _invoiceService.GetInvoicesAsync()).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();
                query = query.Where(i => (i.InvoiceNumber != null && i.InvoiceNumber.ToLower().Contains(search)) ||
                                         (i.CustomerName != null && i.CustomerName.ToLower().Contains(search)));
            }

            DateTime today = DateTime.Today;

            switch (SelectedDateRange)
            {
                case "Today": query = query.Where(i => i.OnDate.Date == today); break;
                case "Yesterday": query = query.Where(i => i.OnDate.Date == today.AddDays(-1)); break;
                case "This Week": query = query.Where(i => i.OnDate.Date >= today.AddDays(-(int)today.DayOfWeek)); break;
                case "This Month": query = query.Where(i => i.OnDate.Month == today.Month && i.OnDate.Year == today.Year); break;
            }

            if (SelectedPaymentMode != "All Modes")
            {
                if (Enum.TryParse<PaymentMode>(SelectedPaymentMode, true, out var parsedEnumMode))
                {
                    var validIds = await _invoiceService.GetPayments(parsedEnumMode);

                    query = query.Where(i => validIds.Contains(i.Id));
                }
                //if (Enum.TryParse<PaymentMode>(SelectedPaymentMode, true, out var parsedEnumMode))
                //{
                //    var validIds = _allPayments
                //         .Where(p => p.PaymentMode == parsedEnumMode)
                //         .Select(p => p.InvoiceId)
                //         .ToHashSet();

                //    query = query.Where(i => validIds.Contains(i.Id));
                //}
            }

            var finalResults = query.ToList();
            FilteredInvoices = new ObservableCollection<Invoice>(finalResults);

            TotalSales = finalResults.Sum(i => i.BillAmount);
            TotalReceived = finalResults.Sum(i => i.PaidAmount);
            TotalPending = finalResults.Sum(i => i.BalanceAmount);
        }

        /// <summary>
        ///  Handle Invoice Select
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private async Task HandleInvoiceSelectionAsync(Invoice invoice)
        {
            // 1. Show the native Action Sheet
            string action = await Application.Current!.Windows[0].Page!.DisplayActionSheetAsync(
                $"Invoice {invoice.InvoiceNumber}", "Cancel", "Delete Invoice", "View Details", "Edit Invoice");

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
                await OpenEditPage(invoice);
            }

            // Deselect the row so it can be clicked again
            SelectedInvoice = null;
        }

        private async Task OpenEditPage(Invoice invoice)
        {
            if (invoice == null) return;

            try
            {
                // Force the navigation onto the Main UI Thread
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync($"EditInvoicePage?InvoiceId={invoice.Id}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Thread dispatch failed: {ex.Message}");
            }
        }


        private async Task OpenInvoiceDetailsModalAsync(Invoice invoice)
        {
            IsBusy = true;
            try
            {
                //var items = await GetContext().InvoiceItems.Where(i => i.InvoiceId == invoice.Id).ToListAsync();
                var items = await _invoiceService.GetInvoiceItemByInvoiceId(invoice.Id);

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
        public async Task ShowScanPopup()
        {
          await  _invoiceService.OpenScanDialogAsync();
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
            bool confirm = await Application.Current!.Windows[0]!.Page!.DisplayAlertAsync(
                "Delete Invoice",
                $"Are you sure you want to permanently delete {invoice.InvoiceNumber}? This will remove all associated items and payments.",
                "Yes, Delete", "Cancel");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                // Enabling Trnascation and Roll back concept
                using var transaction = await GetContext().Database.BeginTransactionAsync();
                try
                {
                    // 2. Perform bulk deletions directly on the database (EF Core 7+)
                    // Note: Replace 'InvoiceItems', 'InvoicePayment', and 'Invoices'
                    // with the actual DbSet property names in your DbContext.

                    await GetContext().InvoiceItems
                        .Where(i => i.InvoiceId == invoice.Id)
                        .ExecuteDeleteAsync();

                    await GetContext().InvoicePayments
                        .Where(p => p.InvoiceId == invoice.Id)
                        .ExecuteDeleteAsync();

                    await GetContext().Invoices
                        .Where(i => i.Id == invoice.Id)
                        .ExecuteDeleteAsync();

                    // 3. Commit the transaction to save changes permanently
                    await transaction.CommitAsync();

                    // If we reach here, the deletion was successful
                    await Application.Current.Windows[0].Page!.DisplayAlertAsync("Deleted", "Invoice deleted successfully.", "OK");

                    // Reload the table
                    await LoadDataAsync();
                }
                catch (Exception)
                {
                    // 4. Roll back the transaction if any database operation fails
                    await transaction.RollbackAsync();

                    // Re-throw the exception so the outer catch block can handle the UI error display
                    throw;
                }
                // Safely delete the invoice and its children in a transaction
                //await db.RunInTransactionAsync(tran =>
                //{
                //    tran.Table<InvoiceItem>().Delete(i => i.InvoiceId == invoice.Id);
                //    tran.Table<PaymentDetail>().Delete(p => p.InvoiceId == invoice.Id);
                //    tran.Delete(invoice);
                //});

                // await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Deleted", "Invoice deleted successfully.", "OK");

                // Reload the table
                //await LoadDataAsync();
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
            await Shell.Current.GoToAsync("InvoiceEntryPage");
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.Windows[0] != null)
                    await Application.Current.Windows[0].Page!.DisplayAlertAsync(title, message, "OK");
            });
        }
    }
}