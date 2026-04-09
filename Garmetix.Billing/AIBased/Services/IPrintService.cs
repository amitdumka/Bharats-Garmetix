using System.Threading.Tasks;

namespace  Garmetix.Billing.AIBased.Services
{
    public interface IPrintService
    {
        Task PrintReceiptAsync(byte[] receiptData);
    }
}