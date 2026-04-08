using Garmetix.Reports.InvoicePrinter.Pages;
using Garmetix.Reports.InvoicePrinter.Services;
using Garmetix.Reports.InvoicePrinter.ViewModels;
using Garmetix.Reports.PageModels;
using Garmetix.Reports.Pages;
using LedgerReportPage = Garmetix.Reports.Pages.LedgerReportPage;

namespace Garmetix.Reports
{
    // All the code in this file is included in all platforms.
    public static class ReportsModule
    {
        public static MauiAppBuilder EnableReports(this MauiAppBuilder builder)
        {
            // Register Services
            builder.Services.AddSingleton<IPrintService, PdfPrintService>();

            // Register Views and ViewModels
            builder.Services.AddSingleton<InvoicePrinterPage>();
            builder.Services.AddSingleton<InvoicePageModel>();

            //Party Ledger
            builder.Services.AddTransient<LedgerReportPageModel>();
            builder.Services.AddTransient<LedgerReportPage>();

            return builder;
        }
    }

    public static class ReportModules
    {
        public static MauiAppBuilder UseReporting(this MauiAppBuilder builder)
        {


            //Page Models
            builder.Services.AddTransient<AccountingPageModel>();
            builder.Services.AddTransient<LedgerReportPageModel>();
            builder.Services.AddTransient<PayrollPageModel>();
            builder.Services.AddTransient<ReprintPageModel>();
            builder.Services.AddTransient<VoucherReportPage>();

            //Pages
            builder.Services.AddTransient<AccountingReportPage>();
            builder.Services.AddTransient<EmployeeReportPage>();
            builder.Services.AddTransient<LedgerReportPage>();
            builder.Services.AddTransient<ReprintReportPage>();
            builder.Services.AddTransient<VoucherReportPage>();

            return builder;
        }
    }
}
