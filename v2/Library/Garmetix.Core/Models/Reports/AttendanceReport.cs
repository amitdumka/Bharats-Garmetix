
using Bharat.ToolKits.Extensions;
using Garmetix.Models.HRM;

namespace Garmetix.Models.Reports
{
    public class AttendanceReport
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Mobile { get; set; }=string.Empty;
        public string Email { get; set; } = string.Empty;
        public EmployeeCategory Department { get; set; }
        public DateTime Date { get; set; }=DateTime.Now;

        public List<AttendanceItem>? Attendances { get; set; }
        public decimal Present
        { get { return (Attendances!.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.PaidLeave || a.Status == AttendanceStatus.Holiday || a.Status == AttendanceStatus.Sunday) + (Attendances.Count(Attendances => Attendances.Status == AttendanceStatus.HalfDay) / 2)); } }
        public decimal Absent { get { return (Attendances!.Count(a => a.Status == AttendanceStatus.Absent || a.Status == AttendanceStatus.CasualLeave || a.Status == AttendanceStatus.OnLeave) + (Attendances.Count(Attendances => Attendances.Status == AttendanceStatus.HalfDay) / 2)); } }
        public int WorkingDays { get { return DateHelper.NoOfWorkingDaysWithoutSaturday(Date); } }
        public int DaysInMonth
        { get { return DateTime.DaysInMonth(Date.Year, Date.Month); } }
    }
}
