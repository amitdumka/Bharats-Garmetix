using System.Collections.Generic;

namespace Garmetix.Commons.Dashboard.Models
{

    /// <summary>
    /// Dashboard Data model, Need to update 
    /// </summary>
    public class DashboardDataModel
    {
        // Metric Cards
        public decimal TodaySales { get; set; }
        public decimal MonthPurchases { get; set; }
        public decimal MonthExpenses { get; set; }
        public decimal TotalCashBalance { get; set; }

        // Charts
        public List<ChartDataPoint> WeeklyCashFlow { get; set; } = new();
        public List<ChartDataPoint> ExpenseDistribution { get; set; } = new();

        // Feed
        public List<RecentTransaction> RecentActivity { get; set; } = new();
    }

    /// <summary>
    /// Chart Data point
    /// </summary>
    public class ChartDataPoint
    {
        public string Label { get; set; }
        public double Value1 { get; set; }
        public double Value2 { get; set; }
    }

    /// <summary>
    /// Recent Transcation
    /// </summary>
    public class RecentTransaction
    {
        public string Type { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string TimeDisplay { get; set; }
        public string ColorHex { get; set; }
        public System.DateTime SortDate { get; set; }
    }
}