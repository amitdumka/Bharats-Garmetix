using Garmetix.CoreBase.HRM.DataModels;
using Garmetix.Models.HRM;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.HRM.PageModels
{
    public class EmployeePageModel : PageModel<Employee>
    {

        public EmployeePageModel()
        {


            DefaultSortedColName = nameof(Employee.JoiningDate);
            DefaultSortedOrder = Ascending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText = nameof(Employee.EmpId), MappingName = nameof(Employee.EmpId) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.FullName), MappingName = nameof(Employee.FullName) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Gender), MappingName = nameof(Employee.Gender) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.PAN), MappingName = nameof(Employee.PAN) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Aadhar), MappingName = nameof(Employee.Aadhar) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Mobile), MappingName = nameof(Employee.Mobile) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.JoiningDate), MappingName = nameof(Employee.JoiningDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Working), MappingName = nameof(Employee.Working) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.DateOfBirth), MappingName = nameof(Employee.DateOfBirth), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Category), MappingName = nameof(Employee.Category) },
                this.GetEditDeleteButtons(),//Add the edit and delete buttons);
            ];

            return GridColumns;
        }
    }

    public class EmployeeDetailPageModel : PageModel<EmployeeDetail>
    {
        public EmployeeDetailPageModel()
        {
            DefaultSortedColName = nameof(EmployeeDetail.Employee.StaffName);
            DefaultSortedOrder = Ascending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText = nameof(Employee.EmpId), MappingName = nameof(Employee.EmpId) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.FullName), MappingName = nameof(Employee.FullName) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Gender), MappingName = nameof(Employee.Gender) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.PAN), MappingName = nameof(Employee.PAN) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Aadhar), MappingName = nameof(Employee.Aadhar) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Mobile), MappingName = nameof(Employee.Mobile) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.JoiningDate), MappingName = nameof(Employee.JoiningDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Working), MappingName = nameof(Employee.Working) },
                new DataGridTextColumn() { HeaderText = nameof(Employee.DateOfBirth), MappingName = nameof(Employee.DateOfBirth), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(Employee.Category), MappingName = nameof(Employee.Category) },
                this.GetEditDeleteButtons(),//Add the edit and delete buttons);
            ];
            return GridColumns;
        }
    }

    public class SalaryPaySlipPageModel : PageModel<SalaryPaySlip>
    {
        public SalaryPaySlipPageModel()
        {
            DefaultSortedColName = nameof(SalaryPaySlip.MonthYear);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                GetColumn(nameof(SalaryPaySlip.MonthYear)),
                GetColumn(nameof(SalaryPaySlip.EmployeeId)),
                GetColumn(nameof(SalaryPaySlip.BasicSalary)),
                GetColumn(nameof(SalaryPaySlip.TotalDeductions)),
                GetColumn(nameof(SalaryPaySlip.TotalEarnings)),
                GetColumn(nameof(SalaryPaySlip.Remarks)),
                GetColumn(nameof(SalaryPaySlip.NetSalary)),
            ];
            return GridColumns;
        }
    }

    public class TimeSheetPageModel : PageModel<TimeSheet>
    {
        public TimeSheetPageModel()
        {
            DefaultSortedColName = nameof(TimeSheet.OutTime);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                // gridColumns.Add(new DataGridTextColumn() { HeaderText = nameof(Voucher.Id), MappingName = nameof(Voucher.Id) });
                new DataGridTextColumn() { HeaderText = nameof(TimeSheet.OutTime), MappingName = nameof(TimeSheet.OutTime) },
                new DataGridTextColumn() { HeaderText = nameof(TimeSheet.InTime), MappingName = nameof(TimeSheet.InTime) },
                new DataGridTextColumn() { HeaderText = nameof(TimeSheet.OutTime), MappingName = nameof(TimeSheet.OutTime) },
                new DataGridTextColumn() { HeaderText = nameof(TimeSheet.Duration), MappingName = nameof(TimeSheet.Duration) },
                new DataGridTextColumn() { HeaderText = nameof(TimeSheet.Reason), MappingName = nameof(TimeSheet.Reason) },
            ];



            return GridColumns;
        }
    }



    public class SalaryPaymentPageModel : PageModel<SalaryPayment>
    {

        public SalaryPaymentPageModel()
        {
            DataModel = new SalaryPaymentDataModel();
            DefaultSortedColName = nameof(SalaryPayment.OnDate);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                new DataGridTextColumn() { HeaderText = "Name", MappingName = "Employee.FullName" },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.SalaryMonth), MappingName = nameof(SalaryPayment.SalaryMonth) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.SalaryComponent), MappingName = nameof(SalaryPayment.SalaryComponent) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.Remarks), MappingName = nameof(SalaryPayment.Remarks) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.Amount), MappingName = nameof(SalaryPayment.Amount) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.OnDate), MappingName = nameof(SalaryPayment.OnDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(SalaryPayment.PaymentMode), MappingName = nameof(SalaryPayment.PaymentMode) },
            ];


            return GridColumns;
        }
    }

    public class SalaryStructurePageModel : PageModel<SalaryStructure>
    {

        public SalaryStructurePageModel()
        {
            DefaultSortedColName = nameof(SalaryStructure.FromDate);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                // gridColumns.Add(new DataGridTextColumn() { HeaderText = nameof(Voucher.Id), MappingName = nameof(Voucher.Id) });
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.BasicSalary), MappingName = nameof(SalaryStructure.BasicSalary) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.HRA), MappingName = nameof(SalaryStructure.HRA) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.YearlyBonus), MappingName = nameof(SalaryStructure.YearlyBonus) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.Deductions), MappingName = nameof(SalaryStructure.Deductions) },
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.FromDate), MappingName = nameof(SalaryStructure.FromDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(SalaryStructure.TotalDeductions), MappingName = nameof(SalaryStructure.TotalDeductions) },
            ];


            return GridColumns;
        }
    }

    public class MonthlyAttendancePageModel : PageModel<MonthlyAttendance>
    {
        public MonthlyAttendancePageModel()
        {
            DefaultSortedColName = nameof(MonthlyAttendance.OnDate);
            DefaultSortedOrder = Descending;
        }
        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
            [
                // gridColumns.Add(new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Id), MappingName = nameof(MonthlyAttendance.Id) });
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Absent), MappingName = nameof(MonthlyAttendance.Absent) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Present), MappingName = nameof(MonthlyAttendance.Present) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.BillableDays), MappingName = nameof(MonthlyAttendance.BillableDays) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.NoOfWorkingDays), MappingName = nameof(MonthlyAttendance.NoOfWorkingDays) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Sunday), MappingName = nameof(MonthlyAttendance.Sunday) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.OnDate), MappingName = nameof(MonthlyAttendance.OnDate), Format = "dd/MMM/yyyy" },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.HalfDay), MappingName = nameof(MonthlyAttendance.HalfDay) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Holidays), MappingName = nameof(MonthlyAttendance.Holidays) },
                new DataGridTextColumn() { HeaderText = nameof(MonthlyAttendance.Remarks), MappingName = nameof(MonthlyAttendance.Remarks) },
            ];


            return GridColumns;
        }
    }
}
