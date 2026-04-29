using Garmetix.Core.Enums;
using Garmetix.Core.VM.Dashboards;
using Garmetix.Databases.Services;
using Garmetix.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Garmetix.CoreServices.Dashboard
{
    // Assuming FinancialInfo, DatabaseService, Db (DbContext), and VoucherType are defined elsewhere.
    // The existing GetPayrollInfoAsync is kept as is.
    /// <summary>
    /// Dashboard Service
    ///
    /// </summary>

    public class DashboardService : BaseServices
    {
        public static PayrollInfo? PayrollInfo { get; set; } = new PayrollInfo();
        public static FinancialInfo? FinancialInfo { get; set; } = new FinancialInfo();

        public static DashboardService? Instance { get; private set; }
        public static DateTime LastUpdated { get; set; } = DateTime.Now.AddDays(-1);

        public DashboardService() : base()
        {
            Instance ??= this;

            PayrollInfo ??= new PayrollInfo();
            FinancialInfo ??= new FinancialInfo();



        }

        public static async Task<bool> RefreshDashBoard()
        {
            if (Instance != null)
            {
                if (FinancialInfo == null && PayrollInfo == null)
                {
                    try
                    {
                        bool flag = false;

                        flag = await GetFinancialInfoAsync();
                        flag = await GetPayrollInfoAsync();
                        if (flag) LastUpdated = DateTime.Now;
                        return flag;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        LastUpdated = DateTime.Now.AddDays(-1);
                        return false;
                    }
                }
                if (DateTime.Now.Subtract(LastUpdated).TotalMinutes < 10) return true;

                try
                {
                    bool flag = false;

                    //if(FinancialInfo == null||) 
                    flag = await GetFinancialInfoAsync();
                    //if(PayrollInfo != null && PayrollInfo.EmployeeList != null && PayrollInfo.EmployeeList.Count > 0)
                    flag = await GetPayrollInfoAsync();
                    if (flag) LastUpdated = DateTime.Now;
                    return flag;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    LastUpdated = DateTime.Now.AddDays(-1);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Asynchronously retrieves financial information with a focus on performance and low memory usage.
        /// This method uses asynchronous database calls to aggregate data directly on the server,
        /// avoiding the need to load entire tables into memory.
        /// </summary>
        /// <returns>True if the data retrieval and processing are successful, false otherwise.</returns>
        public static async Task<bool> GetFinancialInfoAsync()
        {
            try
            {
                var financialInfo = new FinancialInfo();

                // Retrieve CompanyId and StoreId.
                var companyId = DatabaseService.CompanyId;
                var storeId = DatabaseService.StoreId;

                // Define the current date and year for filtering.
                var currentYear = DateTime.Now.Year;
                var currentMonth = DateTime.Now.Month;
                var currentDate = DateTime.Now.Date;

                // --- Performance Optimization: Aggregate data directly on the database server ---
                // This avoids fetching all data for the entire year into memory.

                // Step 1: Calculate sums from Vouchers and CashVouchers asynchronously in parallel
                // Note: We use SumAsync for aggregation and separate queries for different time periods.
                var vouchersTask = Task.WhenAll(
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.OnDate.Month == currentMonth && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.OnDate.Month == currentMonth && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Date == currentDate && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.Vouchers.Where(c => c.StoreId == storeId && c.OnDate.Date == currentDate && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount)
                );

                var cashVouchersTask = Task.WhenAll(
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.OnDate.Month == currentMonth && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Year == currentYear && c.OnDate.Month == currentMonth && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Date == currentDate && c.VoucherType != VoucherType.Receipt).SumAsync(c => c.Amount),
                    Db.CashVouchers.Where(c => c.StoreId == storeId && c.OnDate.Date == currentDate && c.VoucherType == VoucherType.Receipt).SumAsync(c => c.Amount)
                );

                // Wait for both voucher tasks to complete.
                await Task.WhenAll(vouchersTask, cashVouchersTask);

                // Extract the results from the tasks.
                var vouchers = vouchersTask.Result;
                var cashVouchers = cashVouchersTask.Result;

                // Step 2: Combine the results to populate the financial info object.
                financialInfo.TotalExpense = vouchers[0] + cashVouchers[0];
                financialInfo.TotalRecipets = vouchers[1] + cashVouchers[1];
                financialInfo.MonthlyExpense = vouchers[2] + cashVouchers[2];
                financialInfo.MonthlyRecipets = vouchers[3] + cashVouchers[3];
                financialInfo.TodaysExpense = vouchers[4] + cashVouchers[4];
                financialInfo.TodaysRecipets = vouchers[5] + cashVouchers[5];

                // Step 3: Calculate the total receivables.
                financialInfo.TotalReceivable = await Db.CustomerDues
                    .Where(c => c.StoreId == storeId && !c.Paid && c.OnDate.Year == currentYear)
                    .SumAsync(c => c.Amount);

                // TODO: Sales not enabled due to lack of data
                financialInfo.TotalSales = 0;
                financialInfo.MonthlySales = 0;
                financialInfo.TodaysSales = 0;

                // TODO: Payable not enabled due to lack of data
                financialInfo.TotalPayable = 0;

                // Update the static property on success.
                FinancialInfo = financialInfo;
                // LastUpdated = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                //LastUpdated = DateTime.Now.AddDays(-1);
                Debug.WriteLine("Error in GetFinancialInfoAsync: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Asynchronously retrieves payroll attendance information for the current month and populates the PayrollInfo.EmployeeList.
        /// Optimized for performance and low memory/CPU usage by performing aggregations directly on the database.
        /// </summary>
        /// <returns>True if the data retrieval and processing are successful, false otherwise.</returns>
        public static async Task<bool> GetPayrollInfoAsync()
        {
            try
            {
                var storeId = DatabaseService.StoreId;
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var currentDate = DateTime.Now.Date;

                // Optimized Query 1: Get monthly present days per employee directly from DB.
                // This query groups by employee name and counts 'Present' statuses on the database server.
                var monthlyAttendanceSummary = await Db.Attendances.Include(a => a.Employee)
                    .Where(a => a.StoreId == storeId && a.OnDate.Month == currentMonth && a.OnDate.Year == currentYear)
                    .GroupBy(a => a.Employee.FirstName + " " + a.Employee.LastName) // Grouping by FullName (assuming it's unique enough for display)
                    .Select(g => new
                    {
                        EmployeeFullName = g.Key,
                        NoofDayPresent = g.Count(a => a.Status == AttendanceStatus.Present)
                    })
                    .ToListAsync(); // Execute query and materialize results into a list

                // Optimized Query 2: Get today's attendance status for relevant employees directly from DB.
                // This query fetches only the required status for today and stores it in a dictionary for quick lookup.
                var todayAttendanceStatuses = await Db.Attendances.Include(a => a.Employee)
                    .Where(a => a.StoreId == storeId && a.OnDate.Date == currentDate)
                    .Select(a => new
                    {
                        EmployeeFullName = a.Employee.FullName,
                        TodayStatus = a.Status.ToString()
                    })
                    .ToDictionaryAsync(a => a.EmployeeFullName, a => a.TodayStatus); // Efficiently create a dictionary for lookup

                // Combine results in-memory:
                // Iterate through the monthly summary and populate the EmployeeInfo list,
                // using the dictionary to efficiently find today's attendance.
                var employeeInfoList = new List<EmployeeInfo>();
                foreach (var summary in monthlyAttendanceSummary)
                {
                    employeeInfoList.Add(new EmployeeInfo
                    {
                        Name = summary.EmployeeFullName,
                        MonthlySale = 0, // Retained as per original logic (not derived from attendance)
                        NoofDayPresent = summary.NoofDayPresent,
                        // Use GetValueOrDefault for safe access, providing "N/A" if no attendance record for today.
                        TodayAttendance = todayAttendanceStatuses.GetValueOrDefault(summary.EmployeeFullName, "N/A")
                    });
                }

                // Assign the processed list to the static PayrollInfo object.
                PayrollInfo.EmployeeList = employeeInfoList;
                //LastUpdated = DateTime.Now;
                return true;
            }
            catch (Exception ex)
            {
                // LastUpdated = DateTime.Now.AddDays(-1);
                Debug.WriteLine("Error in GetPayrollInfoAsync: " + ex.Message);
                return false;
            }
        }

        // The existing GetPayrollInfoAsync method.
        public static async Task<bool> GetPayrollInfoAsync_Old()
        {
            // ... (The code for this method remains the same as in the previous response) ...
            try
            {
                // Retrieve CompanyId and StoreId.
                var companyId = DatabaseService.CompanyId;
                var storeId = DatabaseService.StoreId;

                // Define the current month and year for filtering.
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var currentDate = DateTime.Now.Date;

                // Asynchronously fetch attendance records for the current month and store.
                // We use .ToListAsync() to execute the query asynchronously.
                var attendances = await Db.Attendances
                    .Include(c => c.Employee)
                    .Where(a => a.OnDate.Month == currentMonth && a.OnDate.Year == currentYear && a.StoreId == storeId)
                    .Select(c => new
                    {
                        c.OnDate,
                        c.Status,
                        c.Employee.FullName
                    })
                    .ToListAsync();

                // Process the retrieved data (this remains synchronous as it's in-memory LINQ).
                var employeeInfoList = attendances
                    .GroupBy(c => c.FullName)
                    .Select(group => new EmployeeInfo
                    {
                        Name = group.Key,
                        MonthlySale = 0,
                        NoofDayPresent = group.Count(a => a.Status == AttendanceStatus.Present),
                        TodayAttendance = group.FirstOrDefault(x => x.OnDate.Date == currentDate)?.Status.ToString() ?? "N/A"
                    })
                    .ToList();

                // Assign the processed list to the PayrollInfo object.
                PayrollInfo.EmployeeList = employeeInfoList;

                return true;
            }
            catch (Exception ex)
            {
                // Log the error using Debug.WriteLine and return false.
                Debug.WriteLine("Error in GetPayrollInfoAsync: " + ex.Message);
                return false;
            }
        }
    }
}