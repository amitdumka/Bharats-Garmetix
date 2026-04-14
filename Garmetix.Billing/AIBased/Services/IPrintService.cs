
namespace  Garmetix.Billing.AIBased.Services
{
    public interface IPrintService
    {
        // Your existing thermal print method
        Task PrintReceiptAsync(byte[] receiptBytes);

        // NEW: Direct HTML Print method
        Task PrintHtmlAsync(string htmlContent, string documentName = "Invoice");
    }
}