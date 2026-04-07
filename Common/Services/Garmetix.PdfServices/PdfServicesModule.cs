using Garmetix.PdfServices.Interfaces;
using Garmetix.PdfServices.Services.Accounting;
using Garmetix.PdfServices.Services.Payroll;

namespace Garmetix.PDFServices
{
    // All the code in this file is included in all platforms.
    public static class PdfServicesModule
    {
        public static MauiAppBuilder EnablePdfServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IPdfVoucherService, PdfVoucherService>();
            builder.Services.AddSingleton<IPdfPayrollService, PdfPayrollService>();
            builder.Services.AddSingleton<IPdfAccountingService, PdfAccountingService>();

            return builder;
        }
    }
}
