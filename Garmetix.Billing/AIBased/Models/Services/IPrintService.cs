using System.Threading.Tasks;

namespace  Garmetix.AI.Billing.Services
{
    public interface IPrintService
    {
        Task PrintReceiptAsync(byte[] receiptData);
    }
}