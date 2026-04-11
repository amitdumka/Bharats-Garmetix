using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string currentDateDisplay;

        // --- METRIC CARDS ---
        [ObservableProperty] private decimal todaySales;
        [ObservableProperty] private decimal monthPurchases;
        [ObservableProperty] private decimal monthExpenses;
        [ObservableProperty] private decimal cashBalance;

        // --- CHART DATA COLLECTIONS ---
        [ObservableProperty] private ObservableCollection<ChartDataPoint> weeklyCashFlow = new();
        [ObservableProperty] private ObservableCollection<ChartDataPoint> expenseDistribution = new();

        // --- RECENT ACTIVITY ---
        [ObservableProperty] private ObservableCollection<RecentTransaction> recentTransactions = new();

        public DashboardViewModel()
        {
            CurrentDateDisplay = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }

        public async Task LoadDashboardAsync()
        {
            IsBusy = true;
            try
            {
                // In a production environment, these would be complex SQLite aggregate queries.
                // For this architectural setup, we are simulating the aggregate results.

                // 1. Load Metric Cards
                TodaySales = 45250.00m;
                MonthPurchases = 128400.00m;
                MonthExpenses = 15200.00m;
                CashBalance = 315600.00m;

                // 2. Load Bar Chart Data (Last 7 Days Cash Flow)
                WeeklyCashFlow = new ObservableCollection<ChartDataPoint>
                {
                    new ChartDataPoint { Label = "Mon", Value1 = 12000, Value2 = 5000 },
                    new ChartDataPoint { Label = "Tue", Value1 = 15500, Value2 = 2000 },
                    new ChartDataPoint { Label = "Wed", Value1 = 9800,  Value2 = 18000 },
                    new ChartDataPoint { Label = "Thu", Value1 = 22000, Value2 = 4000 },
                    new ChartDataPoint { Label = "Fri", Value1 = 31000, Value2 = 12000 },
                    new ChartDataPoint { Label = "Sat", Value1 = 45250, Value2 = 8000 },
                    new ChartDataPoint { Label = "Sun", Value1 = 0,     Value2 = 0 }
                };

                // 3. Load Doughnut Chart Data (Expense Categories)
                ExpenseDistribution = new ObservableCollection<ChartDataPoint>
                {
                    new ChartDataPoint { Label = "Salaries", Value1 = 45 },
                    new ChartDataPoint { Label = "Rent & Bills", Value1 = 30 },
                    new ChartDataPoint { Label = "Logistics", Value1 = 15 },
                    new ChartDataPoint { Label = "Marketing", Value1 = 10 }
                };

                // 4. Load Recent Ledger Activity
                RecentTransactions = new ObservableCollection<RecentTransaction>
                {
                    new RecentTransaction { Type = "SALE", Reference = "INV-1004", Amount = 4500, Time = "10 mins ago", ColorHex = "#10B981" },
                    new RecentTransaction { Type = "EXPENSE", Reference = "Tea & Snacks", Amount = 150, Time = "1 hour ago", ColorHex = "#F43F5E" },
                    new RecentTransaction { Type = "PURCHASE", Reference = "INW-209", Amount = 12500, Time = "3 hours ago", ColorHex = "#8B5CF6" },
                    new RecentTransaction { Type = "RECEIPT", Reference = "UPI - Rahul", Amount = 2000, Time = "5 hours ago", ColorHex = "#0EA5E9" }
                };
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Dashboard failed to load: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RefreshDataAsync() => await LoadDashboardAsync();
    }

    // Helper Models for UI Binding
    public class ChartDataPoint
    {
        public string Label { get; set; }
        public double Value1 { get; set; } // e.g., Inflow or Percentage
        public double Value2 { get; set; } // e.g., Outflow
    }

    public class RecentTransaction
    {
        public string Type { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Time { get; set; }
        public string ColorHex { get; set; }
    }
}