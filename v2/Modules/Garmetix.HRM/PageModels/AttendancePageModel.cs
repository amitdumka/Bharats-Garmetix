using Garmetix.Base.PageModels;
using Garmetix.HRM.DataModels;
using Garmetix.Core.Models.HRM;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.HRM.PageModels
{
    public class AttendancePageModel : PageModel<Attendance>
    {

        public AttendancePageModel()
        {
            DataModel = new AttendanceDataModel();
            DefaultSortedColName = nameof(Attendance.OnDate);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText ="Staff Name", MappingName = "Employee.FullName" },
                new DataGridTextColumn() { HeaderText = nameof(Attendance.OnDate), MappingName = nameof(Attendance.OnDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(Attendance.Status), MappingName = nameof(Attendance.Status) },
                new DataGridTextColumn() { HeaderText = nameof(Attendance.EntryTime), MappingName = nameof(Attendance.EntryTime) },
                new DataGridTextColumn() { HeaderText = nameof(Attendance.Remarks), MappingName = nameof(Attendance.Remarks) },
                this.GetEditDeleteButtons()
            ];

            return GridColumns;
        }
    }
}
