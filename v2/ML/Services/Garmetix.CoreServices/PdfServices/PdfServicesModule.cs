using Garmetix.PdfServices.Services.Payroll;
using Garmetix.Services.Interfaces;
using Garmetix.Services.PdfServices.Services.Accounting;

namespace Garmetix.Core.PdfServices
{
    /// <summary>
    /// Provides extension methods for configuring PDF-related services in a .NET MAUI application. 
    /// </summary>
    /// <remarks>This static class is intended to be used during application startup to register PDF service
    /// dependencies with the dependency injection container. It enables PDF voucher, payroll, and accounting services
    /// for use throughout the application.</remarks>
    public static class PdfServicesModule
    {
        /// <summary>
        /// Configures the application to use PDF-related services for voucher, payroll, and accounting functionality.
        /// </summary>
        /// <remarks>Registers the PDF services as singletons in the dependency injection container. Call
        /// this method during application startup to enable PDF generation features.</remarks>
        /// <param name="builder">The <see cref="MauiAppBuilder"/> instance to configure with PDF services. Cannot be null.</param>
        /// <returns>The same <see cref="MauiAppBuilder"/> instance, enabling method chaining.</returns>
        public static MauiAppBuilder UsePdfServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IPdfVoucherService, PdfVoucherService>();
            builder.Services.AddSingleton<IPdfPayrollService, PdfPayrollService>();
            builder.Services.AddSingleton<IPdfAccountingService, PdfAccountingService>();

            return builder;
        }
    }
}
