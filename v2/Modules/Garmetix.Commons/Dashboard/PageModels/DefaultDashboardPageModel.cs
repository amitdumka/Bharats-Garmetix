using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Commons.Dashboard.Models;
using Garmetix.Core.VM.Dashboards;
using Garmetix.CoreServices.Dashboard;
using System.Diagnostics;

namespace Garmetix.Commons.Dashboard.PageModels
{
    [ObservableRecipient]
    public partial class DefaultDashboardPageModel : ObservableObject
    {
        private readonly DashboardService _service;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private PayrollInfo _payrollInfo;

        [ObservableProperty]
        private FinancialInfo _financialInfo;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _dataLoaded = false;

        [ObservableProperty]
        private bool _isRefreshing = false;

        [ObservableProperty]
        private bool _isBusy = false;

        //Chart Data
        [ObservableProperty]
        private List<ChartData> _weeklySales = [];

        [ObservableProperty]
        private List<ChartData> _weeklyExpenses = [];

        [ObservableProperty]
        private List<ChartData> _weeklyReceipts = [];

        [ObservableProperty]
        private List<Brush> _chartColors = [];

        public DefaultDashboardPageModel(DashboardService dashboardService)
        {
            Title = "Dashboard";
            _service = dashboardService;
            _payrollInfo = new PayrollInfo();
            _financialInfo= new FinancialInfo();
        }

        private void GenerateColors()
        {
            if (ChartColors.Count == 7) return;
            ChartColors.Clear();
            ChartColors.Add(new SolidColorBrush(Colors.Red));
            ChartColors.Add(new SolidColorBrush(Colors.Orange));
            ChartColors.Add(new SolidColorBrush(Colors.Yellow));
            ChartColors.Add(new SolidColorBrush(Colors.Green));
            ChartColors.Add(new SolidColorBrush(Colors.Blue));
            ChartColors.Add(new SolidColorBrush(Colors.Indigo));
            ChartColors.Add(new SolidColorBrush(Colors.Purple));
        }

        private void LoadWeeklySales()
        {
            //  Fetch Weekly Sales
        }

        private void LoadWeeklyExpenses()
        { // Fetch Weekly Expenses
        }

        private void LoadWeeklyReceipts()
        {
            //Fetch Weekly Receipts
        }

        [RelayCommand]
        public async void Appearing()
        {
            if (_isBusy) return;
            _isBusy = true;
            await LoadData();
            _isBusy = false;
        }


        public async Task NavigateToEmployee(EmployeeInfo info)
        {
            //Nagivate to Employee Page
           await Notify.DisplayToastAsync("Not implement yet");
        }

        [RelayCommand]
        public async Task Refresh()
        {
            try
            {
                IsRefreshing = true;
                IsBusy = true;
                await LoadData();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // _errorHandler.HandleError(e);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task LoadData()
        {
            if (IsLoading) return;
            IsLoading = true;
            DataLoaded = await DashboardService.RefreshDashBoard();
            if (DataLoaded)
            {
                PayrollInfo =  DashboardService.PayrollInfo;
                FinancialInfo = DashboardService.FinancialInfo;
            }
            GenerateColors();
            LoadWeeklySales();
            LoadWeeklyExpenses();
            LoadWeeklyReceipts();
            IsLoading = false;
        }
    }
}