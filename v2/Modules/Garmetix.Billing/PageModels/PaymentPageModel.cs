using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.Models.Inventory;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels
{
    internal class PaymentRecord
    {
        public Guid PaymentId { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string Mode { get; set; }
        public decimal Amount { get; set; }
    }

    internal partial class PaymentPageModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;

        // Metric Cards
        [ObservableProperty] private decimal totalCollected;

        [ObservableProperty] private decimal cashTotal;
        [ObservableProperty] private decimal upiTotal;
        [ObservableProperty] private decimal bankCardTotal;

        // The Data Sources
        private List<PaymentRecord> _allPayments = new();

        [ObservableProperty] private ObservableCollection<PaymentRecord> filteredPayments = new();

        // Filters
        [ObservableProperty] private string searchText = string.Empty;

        public List<string> DateRanges { get; } = new() { "Today", "Yesterday", "This Week", "This Month", "All Time" };
        [ObservableProperty] private string selectedDateRange = "Today";

        private partial void OnSearchTextChanged(string value) => ApplyFilters();

        private partial void OnSelectedDateRangeChanged(string value) => ApplyFilters();

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // Load raw tables
                var payments = await db.Table<PaymentDetail>().ToListAsync();
                var invoices = await db.Table<Invoice>().ToListAsync();

                // Create a fast lookup dictionary for invoices
                var invoiceDict = invoices.ToDictionary(i => i.Id, i => i);

                // Join the data into our clean UI Record
                var rawRecords = new List<PaymentRecord>();
                foreach (var p in payments)
                {
                    // Find the invoice this payment belongs to
                    invoiceDict.TryGetValue(p.InvoiceId, out var linkedInvoice);

                    rawRecords.Add(new PaymentRecord
                    {
                        PaymentId = p.Id,
                        Date = (p.PaymentDate == default ? linkedInvoice?.Date ?? DateTime.Now : p.PaymentDate).Value,
                        InvoiceNo = linkedInvoice?.InvoiceNo ?? "Unknown",
                        CustomerName = linkedInvoice?.CustomerName ?? "Unknown / Deleted",
                        Mode = p.Mode ?? "Cash",
                        Amount = p.Amount
                    });
                }

                _allPayments = rawRecords.OrderByDescending(x => x.Date).ToList();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilters()
        {
            var query = _allPayments.AsEnumerable();

            // 1. Text Search Filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.ToLower();
                query = query.Where(p =>
                    p.InvoiceNo.ToLower().Contains(search) ||
                    p.CustomerName.ToLower().Contains(search));
            }

            // 2. Date Filter
            DateTime today = DateTime.Today;
            switch (SelectedDateRange)
            {
                case "Today":
                    query = query.Where(p => p.Date.Date == today);
                    break;

                case "Yesterday":
                    query = query.Where(p => p.Date.Date == today.AddDays(-1));
                    break;

                case "This Week":
                    DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    query = query.Where(p => p.Date.Date >= startOfWeek);
                    break;

                case "This Month":
                    query = query.Where(p => p.Date.Month == today.Month && p.Date.Year == today.Year);
                    break;
            }

            var finalResults = query.ToList();

            // Assign to DataGrid
            FilteredPayments = new ObservableCollection<PaymentRecord>(finalResults);

            // Calculate Metrics
            TotalCollected = finalResults.Sum(p => p.Amount);
            CashTotal = finalResults.Where(p => p.Mode.Equals("Cash", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Amount);
            UpiTotal = finalResults.Where(p => p.Mode.Equals("UPI", StringComparison.OrdinalIgnoreCase)).Sum(p => p.Amount);

            // Assume anything not Cash or UPI is Bank/Card
            BankCardTotal = TotalCollected - CashTotal - UpiTotal;
        }

        [RelayCommand]
        public async Task RefreshAsync() => await LoadDataAsync();
    }
}