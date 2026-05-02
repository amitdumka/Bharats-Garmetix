using Garmetix.Commons.Dashboard.Models;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Commons.Dashboard.Services
{
    /// <summary>
    /// Dashboard Data Service is Create handled
    /// </summary>
    public class DashboardDataService
    {
        // Singleton Instance Pattern
        private static DashboardDataService? _instance;

        public static DashboardDataService Instance => _instance ??= new DashboardDataService();
        protected DatabaseContext _localDb => DatabaseService.Instance.LocalDB;

        // The Cache
        private DashboardDataModel? _cachedData;

        private bool _isCacheValid = false;
        private DateTime _lastFetchTime;

         

        public DatabaseContext GetContext()
        {
            return _localDb;
        }

        // Call this from ANY ViewModel when you save a new Invoice/Purchase!
        public void InvalidateCache()
        {
            _isCacheValid = false;
        }

        public async Task<DashboardDataModel> GetDashboardDataAsync(bool forceRefresh = false)
        {
            // Cache Check: If valid and less than 5 minutes old, return instant RAM data
            if (!forceRefresh && _isCacheValid && _cachedData != null && (DateTime.Now - _lastFetchTime).TotalMinutes < 5)
            {
                return _cachedData;
            }

            var data = new DashboardDataModel();

            // Date Boundaries for SQLite-friendly querying
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            try
            {
                // 1. FETCH AGGREGATES (SQLite Optimized)
                var todaysSales = await GetContext().Invoices.Where(i => i.OnDate >= today).ToListAsync();
                var monthsPurchases = await GetContext().PurchaseInvoices.Where(p => p.InwardDate >= startOfMonth).ToListAsync();

                // Assuming you have an Expense table. If not, this is a placeholder you can map later.
                var monthsExpenses = await GetContext().Vouchers.Where(e => e.OnDate >= startOfMonth && (e.VoucherType == VoucherType.Payment || e.VoucherType == VoucherType.Expense)).ToListAsync();
                var cashmonthsExpenses = await GetContext().CashVouchers.Where(e => e.OnDate >= startOfMonth && (e.VoucherType == VoucherType.Payment || e.VoucherType == VoucherType.Expense)).ToListAsync();

                // Calculate Cards
                data.TodaySales = todaysSales.Sum(i => i.BillAmount);
                data.MonthPurchases = monthsPurchases.Sum(p => p.BillAmount);
                data.MonthExpenses = 0; // Replace with monthsExpenses.Sum(e => e.Amount)

                // Cash Balance = Total Received - Total Purchases - Total Expenses
                var allPayments = await GetContext().InvoicePayments.ToListAsync();

                var allPurchases = await GetContext().PurchaseInvoices.ToListAsync();
                data.TotalCashBalance = allPayments.Sum(p => p.Amount) - allPurchases.Sum(p => p.BillAmount);

                // 2. COMPILE CHART DATA (Weekly Cash Flow)
                var weeklySales = await GetContext().Invoices.Where(i => i.OnDate >= startOfWeek).ToListAsync();
                var weeklyPurchases = await GetContext().PurchaseInvoices.Where(p => p.InwardDate >= startOfWeek).ToListAsync();

                for (int i = 0; i < 7; i++)
                {
                    DateTime targetDay = startOfWeek.AddDays(i);
                    data.WeeklyCashFlow.Add(new ChartDataPoint
                    {
                        Label = targetDay.ToString("ddd"),
                        Value1 = (double)weeklySales.Where(s => s.OnDate.Date == targetDay).Sum(s => s.BillAmount),
                        Value2 = (double)weeklyPurchases.Where(p => p.InwardDate.Date == targetDay).Sum(p => p.BillAmount)
                    });
                }

                // 3. COMPILE RECENT ACTIVITY FEED (Merging Sales and Purchases)
                var recentSales = await GetContext().Invoices.OrderByDescending(i => i.OnDate).Take(5).ToListAsync();
                var recentPurchases = await GetContext().PurchaseInvoices.OrderByDescending(p => p.InwardDate).Take(5).ToListAsync();

                var mixedFeed = new List<RecentTransaction>();

                mixedFeed.AddRange(recentSales.Select(s => new RecentTransaction
                {
                    Type = "SALE",
                    Reference = s.InvoiceNumber,
                    Amount = s.BillAmount,
                    SortDate = s.OnDate,
                    ColorHex = "#10B981",
                    TimeDisplay = GetRelativeTime(s.OnDate)
                }));

                mixedFeed.AddRange(recentPurchases.Select(p => new RecentTransaction
                {
                    Type = "PURCHASE",
                    Reference = p.InwardNumber,
                    Amount = p.BillAmount,
                    SortDate = p.InwardDate,
                    ColorHex = "#8B5CF6",
                    TimeDisplay = GetRelativeTime(p.InwardDate)
                }));

                // Sort the merged list and take top 5
                data.RecentActivity = mixedFeed.OrderByDescending(x => x.SortDate).Take(5).ToList();

                // 4. UPDATE CACHE
                _cachedData = data;
                _lastFetchTime = DateTime.Now;
                _isCacheValid = true;

                return _cachedData;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard Data Error: {ex.Message}");
                return new DashboardDataModel(); // Return empty model on failure to prevent crashes
            }
        }

        // Helper to turn dates into "10 mins ago" or "Yesterday"
        private string GetRelativeTime(DateTime date)
        {
            var span = DateTime.Now - date;
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} mins ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
            if (span.TotalDays < 2) return "Yesterday";
            return date.ToString("dd MMM");
        }
    }
}