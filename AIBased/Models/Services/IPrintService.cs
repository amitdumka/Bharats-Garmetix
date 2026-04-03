using System.Threading.Tasks;

namespace AadwikaBilling.Services
{
    public interface IPrintService
    {
        Task PrintReceiptAsync(byte[] receiptData);
    }
}