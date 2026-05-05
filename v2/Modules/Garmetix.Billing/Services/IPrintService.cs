namespace Garmetix.Billing.Services
{
    /// <summary>
    /// TODO: move to service moudles
    /// </summary>
    public interface IPrintService
    {
        // Your existing thermal print method
        Task PrintReceiptAsync(byte[] receiptBytes);

        // NEW: Direct HTML Print method
        Task PrintHtmlAsync(string htmlContent, string documentName = "Invoice");
    }
}
