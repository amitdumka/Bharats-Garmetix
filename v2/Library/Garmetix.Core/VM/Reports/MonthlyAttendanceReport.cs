namespace Garmetix.Core.VM.Reports
{
    public class MonthlyAttendanceReport : AttendanceReport
    {
        public decimal BillableDays => (decimal)((HalfDay / 2.0m) + 0.0m) + Present + Sunday + PaidLeave + Holiday + 0.0m;
        public decimal PaidLeave { get; set; }
        public decimal CasualLeave { get; set; }
        public int HalfDay { get; set; }
        public int Sunday { get; set; }
        public int WeeklyOff { get; set; }
        public int Holiday { get; set; }

        public decimal NoOfAbsentDays
        {
            get
            {
                return (HalfDay * 0.5m) + Absent + CasualLeave;
            }
        }
    }
}
