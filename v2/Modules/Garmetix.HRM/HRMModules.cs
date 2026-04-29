using Bharat.ToolKits.Helpers;
using Garmetix.HRM.PageModels;
using Garmetix.HRM.Pages.Desktop;
using Garmetix.HRM.Pages.Entry;

namespace Garmetix.HRM
{
    public static class HRMModules
    {
        public static void EnableHRMRoutes()
        {
            RouterHelper.AddRoute(typeof(EntrySalaryPaymentPage));
            RouterHelper.AddRoute(typeof(EntryEmployeePage));
            RouterHelper.AddRoute(typeof(EntryAttendancePage));
            RouterHelper.AddRoute(typeof(EntryTimeSheetPage));
            RouterHelper.AddRoute(typeof(EntrySalaryPaySlipPage));
            RouterHelper.AddRoute(typeof(EntrySalaryStructurePage));
        }

        public static MauiAppBuilder UseHRM(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<EmployeePageModel>();
            builder.Services.AddSingleton<EmployeesPage>();

            builder.Services.AddSingleton<AttendancePageModel>();
            builder.Services.AddSingleton<AttendancePage>();
            builder.Services.AddSingleton<Pages.Mobile.AttendancePage>();

            builder.Services.AddSingleton<SalaryPaymentPageModel>();
            builder.Services.AddSingleton<Pages.Mobile.SalaryPaymentPage>();
            builder.Services.AddSingleton<SalaryPaymentPage>();

            builder.Services.AddSingleton<MonthlyAttendancePageModel>();
            builder.Services.AddSingleton<Pages.Mobile.MonthlyAttendancePage>();
            builder.Services.AddSingleton<MonthlyAttendancePage>();

            builder.Services.AddSingleton<TimeSheetPageModel>();
            builder.Services.AddSingleton<Pages.Mobile.TimeSheetPage>();
            builder.Services.AddSingleton<TimeSheetPage>();

            builder.Services.AddSingleton<SalaryPaySlipPageModel>();
            builder.Services.AddSingleton<SalaryPaySlipPage>();

            builder.Services.AddSingleton<SalaryStructurePageModel>();
            builder.Services.AddSingleton<SalaryStructurePage>();

            // Entry Pages and Form Models
            builder.Services.AddTransient<EmployeeFormModel>();
            builder.Services.AddTransient<EntryEmployeePage>();

            builder.Services.AddTransient<AttendanceFormModel>();
            builder.Services.AddTransient<EntryAttendancePage>();

            builder.Services.AddTransient<SalaryPaymentFormModel>();
            builder.Services.AddTransient<EntrySalaryPaymentPage>();

            builder.Services.AddTransient<TimeSheetFormModel>();
            builder.Services.AddTransient<EntryTimeSheetPage>();

            builder.Services.AddTransient<SalaryPaySlipFormModel>();
            builder.Services.AddTransient<EntrySalaryPaySlipPage>();

            builder.Services.AddTransient<SalaryStructureFormModel>();
            builder.Services.AddTransient<EntrySalaryStructurePage>();
            return builder;
        }
    }
}