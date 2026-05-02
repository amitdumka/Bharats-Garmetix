namespace Garmetix.Models.Dashboards
{
    public class PayrollInfo
    {
        public List<EmployeeInfo> EmployeeList { get; set; } = new List<EmployeeInfo>();
    }
    public class EmployeeInfo
    {
        public string Name { get; set; } = string.Empty;
        public string TodayAttendance { get; set; } = "Absent";
        public decimal NoofDayPresent { get; set; } = 0;
        public decimal MonthlySale { get; set; } = 0;
    }

    public class FinancialInfo
    {
        public decimal TotalSales { get; set; } = 0;
        public decimal TotalExpense { get; set; } = 0;
        public decimal TotalRecipets { get; set; } = 0;

        public decimal MonthlySales { get; set; } = 0;
        public decimal MonthlyExpense { get; set; } = 0;
        public decimal MonthlyRecipets { get; set; } = 0;

        public decimal TodaysSales { get; set; } = 0;
        public decimal TodaysExpense { get; set; } = 0;
        public decimal TodaysRecipets { get; set; } = 0;


        public decimal TotalPayable { get; set; } = 0;
        public decimal TotalReceivable { get; set; } = 0;
    }
}
