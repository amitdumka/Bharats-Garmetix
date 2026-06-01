
namespace Garmetix.RemoteReceiver.Services
{
    public interface IPlatformPrinter
    {
        Task PrintRawPayloadAsync(byte[] data);
    }
}