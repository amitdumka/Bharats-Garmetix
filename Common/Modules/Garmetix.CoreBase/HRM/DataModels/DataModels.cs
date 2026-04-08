using Garmetix.Models.HRM;

namespace Garmetix.CoreBase.HRM.DataModels
{
    public class EmployeeDataModel : DataModel<Employee>
    {
    }

    public class AttendanceDataModel : DataModel<Attendance>
    {
        //public override async Task<List<Attendance>?> GetAllAsync(int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        if (CanViewAll)
        //        {
        //            var pagedData = await _localDb.Set<Attendance>().Include(c=>c.Employee).AsNoTracking().ToListAsync();
        //            return pagedData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        }
        //        else
        //        {
        //            IsError = true;
        //            ErrorMessage = "Not Authozised to access!";
        //            SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
        //            return [];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        IsError = true;
        //        ErrorMessage = ex.Message;
        //        SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
        //        SentrySdk.CaptureMessage("Error: GetAll-Paged:\n" + ex.Message);
        //        SentrySdk.CaptureException(ex);

        //        return [];
        //    }
        //}
        //public override async Task<List<Attendance>?> GetAllAsync()
        //{
        //    try
        //    {
        //        if (CanViewAll)
        //        {
        //            return await _localDb.Attendances.Include(c=>c.Employee).AsNoTracking().ToListAsync();
        //        }
        //        else
        //        {
        //            IsError = true;
        //            ErrorMessage = "Not Authozised to access!";
        //            SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
        //            return [];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        IsError = true;
        //        ErrorMessage = ex.Message;
        //        SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
        //        SentrySdk.CaptureMessage("Error: GetAll:\n" + ex.Message);
        //        SentrySdk.CaptureException(ex);

        //        return [];
        //    }
        //}
    }

    public class EmployeeDetailDataModel : DataModel<EmployeeDetail>
    {
    }

    public class SalaryPaySlipDataModel : DataModel<SalaryPaySlip>
    {
    }

    public class TimeSheetDataModel : DataModel<TimeSheet>
    {
    }

    public class SalaryPaymentDataModel : DataModel<SalaryPayment>
    {
        //public override async Task<List<SalaryPayment>?> GetAllAsync(int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        if (CanViewAll)
        //        {
        //            var pagedData = await _localDb.Set<SalaryPayment>().Include(c => c.Employee).AsNoTracking().ToListAsync();
        //            return pagedData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        }
        //        else
        //        {
        //            IsError = true;
        //            ErrorMessage = "Not Authozised to access!";
        //            SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
        //            return [];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        IsError = true;
        //        ErrorMessage = ex.Message;
        //        SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
        //        SentrySdk.CaptureMessage("Error: GetAll-Paged:\n" + ex.Message);
        //        SentrySdk.CaptureException(ex);

        //        return [];
        //    }
        //}
        //public override async Task<List<SalaryPayment>?> GetAllAsync()
        //{
        //    try
        //    {
        //        if (CanViewAll)
        //        {
        //            return await _localDb.SalaryPayments.Include(c => c.Employee).AsNoTracking().ToListAsync();
        //        }
        //        else
        //        {
        //            IsError = true;
        //            ErrorMessage = "Not Authozised to access!";
        //            SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
        //            return [];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        IsError = true;
        //        ErrorMessage = ex.Message;
        //        SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
        //        SentrySdk.CaptureMessage("Error: GetAll:\n" + ex.Message);
        //        SentrySdk.CaptureException(ex);

        //        return [];
        //    }
        //}

    }

    public class SalaryStructureDataModel : DataModel<SalaryStructure>
    {
    }

    public class MonthlyAttendanceDataModel : DataModel<MonthlyAttendance>
    {
    }
}