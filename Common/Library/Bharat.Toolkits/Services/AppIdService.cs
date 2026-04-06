using Bharat.ToolKits.Helpers;

namespace Bharat.ToolKits.Services
{
    public interface IDeviceIdProvider
    {
        string? GetDeviceId();
    }
    public static class AppIdService
    {
        private const string AppIdKey = "AppId";

        public static Guid GetAppId()
        {
            var id = StorageOps.GetPref(AppIdKey, Guid.Empty);
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                StorageOps.SetPref(AppIdKey, id);
            }
            return id;
        }
    }
}