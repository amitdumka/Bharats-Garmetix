using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Commons.Dashboard.Models;
using Garmetix.Commons.Dashboard.Services;
using System.Collections.ObjectModel;

namespace Garmetix.Commons.Dashboard.PageModels
{
    /// <summary>
    /// Dashboard ViewModels
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string currentDateDisplay;

        // Metric Cards
        [ObservableProperty] private decimal todaySales;
        [ObservableProperty] private decimal monthPurchases;
        [ObservableProperty] private decimal monthExpenses;
        [ObservableProperty] private decimal cashBalance;

        // Charts & Feeds
        [ObservableProperty] private ObservableCollection<ChartDataPoint> weeklyCashFlow = [];
        [ObservableProperty] private ObservableCollection<ChartDataPoint> expenseDistribution = [];
        [ObservableProperty] private ObservableCollection<RecentTransaction> recentTransactions = [];

        public DashboardViewModel()
        {
            CurrentDateDisplay = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
        }

        public async Task LoadDashboardAsync(bool forceRefresh = false)
        {
            IsBusy = true;
            try
            {
                // Instantly grabs data from RAM Cache, or fetches if dirty
                var data = await DashboardDataService.Instance.GetDashboardDataAsync(forceRefresh);

                // Populate UI
                TodaySales = data.TodaySales;
                MonthPurchases = data.MonthPurchases;
                MonthExpenses = data.MonthExpenses;
                CashBalance = data.TotalCashBalance;

                WeeklyCashFlow = new ObservableCollection<ChartDataPoint>(data.WeeklyCashFlow);
                ExpenseDistribution = new ObservableCollection<ChartDataPoint>(data.ExpenseDistribution);
                RecentTransactions = new ObservableCollection<RecentTransaction>(data.RecentActivity);
            }
            catch (Exception ex)
            {
                await Application.Current?.Windows[0]?.Page?.DisplayAlertAsync("Error", $"Dashboard failed to load: {ex?.Message??" "}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task RefreshDataAsync()
        {
            // Forces a hard database read when the user clicks the "Refresh" button
            await LoadDashboardAsync(forceRefresh: true);
        }
    }
}