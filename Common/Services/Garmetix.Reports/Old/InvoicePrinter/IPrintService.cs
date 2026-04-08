// Services/IPrintService.cs
using Garmetix.Reports.InvoicePrinter.Models;

namespace Garmetix.Reports.InvoicePrinter.Services
{
    public interface IPrintService
    {
        Task CreateAndPrintInvoiceAsync(InvoiceModel invoice);
    }
}