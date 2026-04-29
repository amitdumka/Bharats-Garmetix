using Bharat.ToolKits.Extensions;
using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using Garmetix.Core.Enums;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.HRM;
using Garmetix.Core.Models.Inventory;
using Garmetix.Core.Sessions;
using Garmetix.CoreServices.Accounting;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.Reports;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Garmetix.CoreServices.Payroll
{
    public class PayrollServices
    {
        // This class can be used to define services related to the Template module.
        // Currently, it is empty and can be extended as needed.

        public static DatabaseContext Db => DatabaseService.Instance.LocalDB;

        public static async Task<string> GetEmployeeName(Guid id) => (await Db.Employees.FindAsync(id))?.FullName ?? "";

        /// <summary>
        /// Check for duplicate attendance
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        public static bool DuplicateAttendaceCheck(Attendance attendance)
        {
            return Db.Attendances.Any(a => a.OnDate.Date == attendance.OnDate.Date && a.EmployeeId == attendance.EmployeeId);
        }

        /// <summary>
        /// Generates a unique salary payment voucher number for the specified date.
        /// </summary>
        /// <remarks>The voucher number is constructed using the company store code, the year and month of
        /// the specified date,  and a sequential suffix based on the count of salary payments for the given
        /// month.</remarks>
        /// <param name="date">The date for which the voucher number is generated. The year and month of the date are used to create the
        /// voucher number.</param>
        /// <return>A string representing the salary payment voucher number in the format   "{companystorecode}-SPY-{year}{month}-{suffix}", where {suffix} is a zero-padded sequence number.</return>
        public static async Task<string> GetSalaryPaymentVouherNumber(DateTime date)
        {
            string voucherNumber = "";
            string suffix = AccountingServices.AddZerosAsSuffix(Db.SalaryPayments.Where(x => x.OnDate.Year == date.Year && x.OnDate.Month == date.Month).Count() + 1, 4);

            voucherNumber = SessionService.CompanyStoreCode() + "-SPY-" + date.ToString("yyyyMM") + "-" + suffix;

            return voucherNumber;
        }

        /// <summary>
        /// Get Current Salary Structure
        /// </summary>
        /// <param name="month"></param>
        /// <param name="structures"></param>
        /// <returns></returns>
        public static SalaryStructure? CurrentSalaryStructure(DateTime month, ref List<SalaryStructure> structures)
        {
            if (structures ==null|| structures.Count is  0) // Check for null or zero count
            {
                return null;
            }

            var currentStructure = structures.Where(a => a.FromDate <= month && a.ToDate >= month).FirstOrDefault();

            return currentStructure;
        }

        /// <summary>
        ///     Generate PaySlip
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static async Task<SalaryPaySlip?> GeneratePayslip(Guid empid, DateTime month)
        {
            
            var att = Db.MonthlyAttendances.Where(a => a.OnDate.Month == month.Month && a.OnDate.Year == month.Year && a.EmployeeId == empid).FirstOrDefault();
            att ??= await GenerateMonthlyAttendance(empid, month);
            
            if(att == null)
            {
                return null;
            }
            // Fetch Salary Structure
            var salaryStructures = await Db.SalaryStructures.Where(a => a.EmployeeId == empid).ToListAsync();
            // Fetch Current Valid Salary Structure
            var current = CurrentSalaryStructure(month, ref salaryStructures);

            var sundayCount = DateHelper.AllSunday(month);
            int workingDays = DateHelper.NoOfWorkingDays(month);
            decimal amount = att.NoOfAbsentDays * (current?.BasicSalary / 26 ?? 0);

            //if (DateTime.DaysInMonth(month.Year, month.Month) == 31)
            //{
            //}
            //else if (DateTime.DaysInMonth(month.Year, month.Month) == 30)
            //{
            //}
            //else if (DateTime.DaysInMonth(month.Year, month.Month) == 28)
            //{
            //}
            //else if (DateTime.DaysInMonth(month.Year, month.Month) == 29)
            //{
            //}
            // Basic Salary Version
            //TODO: Make it industry specific and indian law compliant
            SalaryPaySlip salaryPaySlip = new()
            {
                CompanyId = DatabaseService.CompanyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "AutoAdmin",
                Deleted = false,
                EmployeeId = empid,
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
                Synced = false,
                Remarks = "Payslip Generated",
                MonthYear = month.Month + "/" + month.Year,
                PayPeriodStart = new DateTime(month.Year, month.Month, 1),
                PayPeriodEnd = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)),
                Gratuity = 0,
                IncomeTax = 0,
                ProfessionalTax = 0,
                HRA = 0,
                ConveyanceAllowance = 0,
                OtherDeductions = 0,
                OtherEarnings = 0,
                SpecialAllowance = 0,
                ProvidentFund = 0,
                Deductions = 0,
                Incentives = 0,
                BasicSalary = current?.BasicSalary - amount ?? (-amount),
            };

            _ = Db.SalaryPaySlips.Add(salaryPaySlip);
            var result = await Db.SaveChangesAsync();
            return result > 0 ? salaryPaySlip : null;
        }

        /// <summary>
        /// Generate Monthly attendance
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static async Task<MonthlyAttendance?> GenerateMonthlyAttendance(Guid empid, DateTime month)
        {
            try
            {
                var attendances = Db.Attendances.Where(a => a.OnDate.Month == month.Month && a.OnDate.Year == month.Year && a.EmployeeId == empid)
                    .Select(c => new { c.OnDate, c.Status })
                    .ToList();

                //Calulate total working days
                MonthlyAttendance monthlyAttendance = new()
                {
                    CompanyId = DatabaseService.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AutoAdmin",
                    Deleted = false,
                    EmployeeId = empid,
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                    OnDate = month,
                    StoreId = DatabaseService.StoreId,
                    Synced = false,
                    StoreGroupId = DatabaseService.StoreGroupId,
                    NoOfWorkingDays = DateHelper.NoOfWorkingDays(month),
                    Present = attendances.Count(a => a.Status == AttendanceStatus.Present),
                    HalfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay),
                    Absent = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                    CasualLeave = attendances.Count(a => a.Status == AttendanceStatus.CasualLeave),
                    Holidays = attendances.Count(a => a.Status == AttendanceStatus.Holiday),
                    PaidLeave = attendances.Count(a => a.Status == AttendanceStatus.PaidLeave),
                    Sunday = attendances.Count(a => a.Status == AttendanceStatus.Sunday),
                    Remarks = "Monthly Attendance Generated",
                    WeeklyLeave = attendances.Count(a => a.Status == AttendanceStatus.SundayHoliday),
                };

                _ = Db.MonthlyAttendances.Add(monthlyAttendance);
                int result = await Db.SaveChangesAsync();
                return result > 0 ? monthlyAttendance : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _ = Notify.DisplayNotificationAsync("Error :" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Run Monthly function
        /// </summary>
        /// <returns></returns>
        public static async Task RunMonthlyFunctionAsync()
        {
            //TODO: make notification and email of this event and make log entry
            if (DateTime.Now.Day != 1)
            {
                return;
            }
            // Generating Monthly Attendance.
            var empids = await Db.Employees.Where(c => c.Working).Select(c => c.Id).ToListAsync();

            foreach (var id in empids)
            {
                await GenerateMonthlyAttendance(id, DateTime.Now.AddMonths(-1));
                _ = GeneratePayslip(id, DateTime.Now.AddMonths(1));
            }
        }

        /// <summary>
        ///  Add Saleman if employee is salesman
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static async Task<bool> AddSalesman(Employee employee)
        {
            if (employee == null || employee.Category != EmployeeCategory.Salesman)
            {
                return false;
            }

            var salesman = new Salesman
            {
                Id = employee.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "AutoAdmin",
                Deleted = false,
                EmployeeId = employee.Id,
                StoreId = DatabaseService.StoreId,
                StoreGroupId = DatabaseService.StoreGroupId,
                Synced = false,
                UpdatedAt = DateTime.UtcNow,
                Active = true,
                CompanyId = employee.CompanyId,
                Name = employee.FullName
            };
            _ = Db.Salesmen.Add(salesman);
            Notify.LogInfoAsync("Salesman Added");
            return await Db.SaveChangesAsync() > 0;
        }

        #region ShareNPrint

        public static PDfFileDetails LastSalaryPaymentDetails { get; set; } = new();
        public static PDfFileDetails LastPaySlipDetails { get; set; } = new();

        public static async Task<bool> PrintSalaryPayment(SalaryPayment salaryPayment, bool isNew = false)
        {
            if (salaryPayment == null)
            {
                return false;
            }

            SalaryPaymentDetails details = new()
            {
                AuthorizedSignatory = SessionService.CompanyName() ?? "",
                CompanyAddress = SessionService.StoreAddress() ?? "",
                CompanyName = SessionService.CompanyName() ?? "",
                Amount = salaryPayment.Amount,
                Narration = salaryPayment.Remarks ?? "",
                VouherNumber = salaryPayment.VoucherNumber,
                CompanyEmail = SessionService.Email() ?? "",
                CompanyPhone = SessionService.Phone() ?? "",
                Date = salaryPayment.OnDate,
                Gstin = SessionService.GstNumber() ?? "",
                PaymentMode = salaryPayment.PaymentMode.ToString(),
                StaffName = salaryPayment.Employee?.FullName ?? "",
                OnAccount = salaryPayment.SalaryComponent.ToString(),
                AmountInWords = NumberToWords.ConvertAmount((double)salaryPayment.Amount),
                Period = NumberToWords.ConvertMonthYearToString(salaryPayment.SalaryMonth),
            };
            var filename = await ServiceHelper.Current.GetService<IPdfPayrollService>()?.CreatePdfSalaryPaymentAsync(details, isNew)! ?? "";
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            LastSalaryPaymentDetails = new PDfFileDetails { FileName = filename, ModelName = "Salary Payment", Naration = $"{details.StaffName}, Voucher Number :{details.VouherNumber} {details.Date:dd/MM/yyyy} Under {salaryPayment.SalaryComponent} For {NumberToWords.ConvertMonthYearToString(salaryPayment.SalaryMonth)} Paid Amount of Rs.{salaryPayment.Amount}, In Words {details.AmountInWords}", };

            return true;
        }

        public static async Task<bool> PrintPaySlip(SalaryPaySlip slip)
        {
            if (slip == null)
            {
                return false;
            }

            PaySlipDetails details = new()
            {
                Gstin = SessionService.GstNumber() ?? "",
                CompanyName = SessionService.CompanyName() ?? "",
                CompanyAddress = SessionService.StoreAddress() ?? "",
                CompanyPhone = SessionService.Phone() ?? "",
                CompanyEmail = SessionService.Email() ?? "",
                StaffName = await GetEmployeeName(slip.EmployeeId),
                Period = slip.MonthYear,
                BasicSalary = slip.BasicSalary,
                Hra = slip.HRA,
                Incentives = slip.Incentives,
                NetSalary = slip.NetSalary,
                TotalDeductions = slip.Deductions,
                TotalEarnings = slip.TotalEarnings,
            };

            var filename = await ServiceHelper.Current.GetService<IPdfPayrollService>()?.CreatePdfPaySlip(details)! ?? "";
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            LastPaySlipDetails = new PDfFileDetails
            {
                FileName = filename,
                ModelName = "Pay Slip",
                Naration = $"Staff Name ={details.StaffName} payslip generated for period {details.Period}  ",
            };

            return true;
        }

        public static async Task<bool> Share(bool isSalaryPayment = false, bool isPaySlip = false)
        {
            if (string.IsNullOrEmpty(LastSalaryPaymentDetails.FileName) || LastSalaryPaymentDetails == null)
            {
                return false;
            }

            try
            {
                var title = "";
                var file = "";
                if (isSalaryPayment)
                {
                    if (LastSalaryPaymentDetails == null || string.IsNullOrEmpty(LastSalaryPaymentDetails.FileName))
                    {
                        return false;
                    }

                    file = LastSalaryPaymentDetails.FileName;
                    title = $"{LastSalaryPaymentDetails.ModelName} {LastSalaryPaymentDetails.Naration}";
                }
                else if (isPaySlip)
                {
                    if (LastPaySlipDetails == null || string.IsNullOrEmpty(LastPaySlipDetails.FileName))
                    {
                        return false;
                    }

                    file = LastPaySlipDetails.FileName;
                    title = $"{LastPaySlipDetails.ModelName} {LastPaySlipDetails.Naration}";
                }

                var _share = ServiceHelper.Current.GetService<IShare>();
                
                if (_share != null)
                {
                    await _share.RequestAsync(new ShareFileRequest
                    {
                        Title = title,
                        File = new ShareFile(file)
                    });
                }
                else
                {
                    // If sharing is not supported, show an alert to the user.
                    await Shell.Current.DisplayAlertAsync("Sharing Not Supported", "Sharing is not supported on this device.", "OK");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Could not share file: {ex.Message}", "OK");
                return false;
            }
        }

        public static async Task<bool> ShareOverEmail(string emailid, bool isSalaryPayment = false, bool isPaySlip = false)
        {
            string naration = "";
            string modelName = "";
            string fileName = "";
            if (isSalaryPayment)
            {
                if (LastSalaryPaymentDetails == null || string.IsNullOrEmpty(LastSalaryPaymentDetails.FileName))
                {
                    return false;
                }

                fileName = LastSalaryPaymentDetails.FileName;
                modelName = LastSalaryPaymentDetails.ModelName;
                naration = LastSalaryPaymentDetails.Naration;
            }
            else if (isPaySlip)
            {
                if (LastPaySlipDetails == null || string.IsNullOrEmpty(LastPaySlipDetails.FileName))
                {
                    return false;
                }

                fileName = LastPaySlipDetails.FileName;
                modelName = LastPaySlipDetails.ModelName;
                naration = LastPaySlipDetails.Naration;
            }

            if (string.IsNullOrEmpty(LastSalaryPaymentDetails.FileName) || LastSalaryPaymentDetails == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailid))
            {
                await Shell.Current.DisplayAlertAsync("Email", "Sharing over to default company email.", "OK");
                emailid = SessionService.Email()??"aadwiafashion@gmail.com";
                //return false;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = $"From {SessionService.CompanyName()},  {modelName} {naration}",
                    Body = $"Please find the attached {modelName} {naration}.\n \nBest regards,\n{SessionService.CompanyName()}\n{SessionService.StoreAddress()}\n{SessionService.Phone()}",
                    To = [emailid], Attachments = []
                };

                message.Attachments.Add(new EmailAttachment(fileName));
                ServiceHelper.Current.GetService<IEmail>()?.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException)
            {
                await Shell.Current.DisplayAlertAsync("Not Supported", "Email is not supported on this device.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to send email: {ex.Message}", "OK");
                return false;
            }
        }

        #endregion ShareNPrint
    }
}