using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers;
using SQLite;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.AIBased.ViewModels
{
    public partial class InvoiceHistoryViewModel : ObservableObject
    {
        private SQLiteAsyncConnection _database;
        private List<Invoice>? _allInvoices = new();
        private List<PaymentDetail>? _allPayments = new();

        [ObservableProperty] private bool isBusy;

        // --- SUMMARY STATS ---
        [ObservableProperty] private decimal totalSales;
        [ObservableProperty] private decimal totalReceived;
        [ObservableProperty] private decimal totalPending;

        // --- THE DATA GRID SOURCE ---
        public ObservableCollection<Invoice> FilteredInvoices { get; set; } = new();

        // --- SEARCH AND FILTERS ---
        [ObservableProperty] private string searchText = string.Empty;

        public List<string> DateRanges { get; } = new()
        {
            "All Time", "Today", "Yesterday", "This Week", "Last Week", "This Month", "Last Month", "This Year"
        };
        [ObservableProperty] private string selectedDateRange = "This Month";

        public List<string> PaymentModes { get; } = new()
        {
            "All Modes", "Cash", "UPI", "Card", "Bank Transfer"
        };
        [ObservableProperty] private string selectedPaymentMode = "All Modes";

        public InvoiceHistoryViewModel()
        {
            // FORCE the use of the v2 database to guarantee no schema clashes
            string dbPath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, "aadwikabilling_v2.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        // Triggers the filter engine whenever the user types or changes a dropdown
        partial void OnSearchTextChanged(string value) => ApplyFilters();
        partial void OnSelectedDateRangeChanged(string value) => ApplyFilters();
        partial void OnSelectedPaymentModeChanged(string value) => ApplyFilters();
        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // 1. Ensure tables exist
               // await _database.CreateTableAsync<Invoice>();
               // await _database.CreateTableAsync<PaymentDetail>();

                _database=await DatabaseHelper.InitializeDatabaseAsync();
                if(_database == null)
                {
                    throw new Exception("Failed to initialize database connection.");
                }
                // 2. CRITICAL FIX: Fetch raw data FIRST, then sort it in C# memory. 
                // This prevents the SQLite LINQ translator from crashing.
                var rawInvoices = await _database.Table<Invoice>().ToListAsync();
                if(rawInvoices == null)
                {
                    throw new Exception("Failed to fetch invoices from the database.");
                }
                _allInvoices = rawInvoices.OrderByDescending(i => i.Date).ToList();

                _allPayments = await _database.Table<PaymentDetail>().ToListAsync();

                // 3. Update UI safely on the Main Thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ApplyFilters();
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Database Error", $"Details: {ex.Message}", "OK");
                    }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
         

        private void ApplyFilters()
        {
            try
            {

                var query = _allInvoices.AsEnumerable();

                // 1. Apply Search Text (Invoice No or Customer Name)
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    string search = SearchText.ToLower();
                    query = query.Where(i =>
                        (i.InvoiceNo != null && i.InvoiceNo.ToLower().Contains(search)) ||
                        (i.CustomerName != null && i.CustomerName.ToLower().Contains(search)));
                }

                // 2. Apply Date Filter
                DateTime today = DateTime.Today;
                switch (SelectedDateRange)
                {
                    case "Today":
                        query = query.Where(i => i.Date.Date == today);
                        break;
                    case "Yesterday":
                        query = query.Where(i => i.Date.Date == today.AddDays(-1));
                        break;
                    case "This Week":
                        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                        query = query.Where(i => i.Date.Date >= startOfWeek);
                        break;
                    case "Last Week":
                        DateTime startOfLastWeek = today.AddDays(-(int)today.DayOfWeek - 7);
                        DateTime endOfLastWeek = startOfLastWeek.AddDays(6);
                        query = query.Where(i => i.Date.Date >= startOfLastWeek && i.Date.Date <= endOfLastWeek);
                        break;
                    case "This Month":
                        query = query.Where(i => i.Date.Month == today.Month && i.Date.Year == today.Year);
                        break;
                    case "Last Month":
                        DateTime lastMonth = today.AddMonths(-1);
                        query = query.Where(i => i.Date.Month == lastMonth.Month && i.Date.Year == lastMonth.Year);
                        break;
                    case "This Year":
                        query = query.Where(i => i.Date.Year == today.Year);
                        break;
                }

                // 3. Apply Payment Mode Filter
                if (SelectedPaymentMode != "All Modes")
                {
                    // Find all Invoice IDs that have a matching payment type in the Payments table
                    var validInvoiceIds = _allPayments
                        .Where(p => p.Mode.Equals(SelectedPaymentMode, StringComparison.OrdinalIgnoreCase))
                        .Select(p => p.InvoiceId)
                        .ToHashSet();

                    query = query.Where(i => validInvoiceIds.Contains(i.Id));
                }

                // 4. Execute and Update UI
                var finalResults = query.ToList();

                FilteredInvoices.Clear();
                foreach (var invoice in finalResults)
                {
                    FilteredInvoices.Add(invoice);
                }

                // 5. Update Summary Dashboards
                TotalSales = finalResults.Sum(i => i.GrandTotal);
                TotalReceived = finalResults.Sum(i => i.PaidAmount);
                TotalPending = finalResults.Sum(i => i.BalanceAmount);
            }
            catch (Exception ex)
            {
                // Safely show the error without crashing
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Database Error", $"Could not load history: {ex.Message}", "OK");
                    }
                });
            }

        }
    }
}