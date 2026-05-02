using Google.Apis.Json;
using Google.Apis.Util.Store;
using System.Threading.Tasks;

namespace Bharat.GoogleDrive.Services
{
    /// <summary>
    /// A custom data store for Google API credentials that uses .NET MAUI's SecureStorage.
    /// This ensures that the user's OAuth tokens are stored securely on the device.
    /// </summary>
    public class MauiSecureDataStore : IDataStore
    {
        public async Task StoreAsync<T>(string key, T value)
        {
            var json = NewtonsoftJsonSerializer.Instance.Serialize(value);
            await SecureStorage.Default.SetAsync(key, json);
        }

        public async Task DeleteAsync<T>(string key)
        {
            SecureStorage.Default.Remove(key);
            await Task.CompletedTask;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await SecureStorage.Default.GetAsync(key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return NewtonsoftJsonSerializer.Instance.Deserialize<T>(json);
        }

        public Task ClearAsync()
        {
            SecureStorage.Default.RemoveAll();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Generates a secure key for storing data, specific to this application.
        /// </summary>
        /// <param name="key">The original key.</param>
        /// <param name="type">The type of the data being stored.</param>
        /// <returns>A securely prefixed key.</returns>
        public static string GenerateStoredKey(string key, Type type)
        {
            return $"{type.FullName}-{key}";
        }
    }
}
