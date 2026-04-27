using System.Diagnostics;

namespace Bharat.ToolKits.Helpers
{
    public class StorageOps
    {
        //Preferences.Default.Remove("first_name");
        //Preferences.Default.Clear();

        public static bool HasPref(string key)
        {
            return Preferences.Default.ContainsKey(key);
        }

        public static void SetPref<T>(T value)
        {
            try
            {
                // Set a string value:
                if (value is Guid guidvalue)
                {
                    Preferences.Default.Set(nameof(value), value.ToString());
                }
                else if (value is Guid?)
                {
                    Preferences.Default.Set(nameof(value), string.Empty);
                }
                else if (value is Enum)
                {
                    Preferences.Default.Set(nameof(value), value.ToString());
                }
                else if (value is bool? || value is bool)
                {
                    if (value == null)
                    {
                        Preferences.Default.Set(nameof(value), false);
                    }
                    else
                    {
                        Preferences.Default.Set(nameof(value), value);
                    }
                }
                else
                {
                    Preferences.Default.Set(nameof(value), value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
                //_ = AksNotify.NotifyAsync($"Error:KeyName {nameof(value)}, {typeof(T)} : {ex.Message}");
            }
        }

        public static void SetPref<T>(string key, T value)
        {
            try
            {
                // Set a string value:
                if (value is Guid guidvalue)
                {
                    Preferences.Default.Set(key, value.ToString());
                }
                else if (value is Guid?)
                {
                    Preferences.Default.Set(key, string.Empty);
                }
                else if (value is Enum)
                {
                    Preferences.Default.Set(key, value.ToString());
                }
                else if (value is bool? || value is bool)
                {
                    if (value == null)
                    {
                        Preferences.Default.Set(key, false);
                    }
                    else
                    {
                        Preferences.Default.Set(key, value);
                    }
                }
                else
                {
                    Preferences.Default.Set(key, value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
               // _ = AksNotify.NotifyAsync($"Error:KeyName {key}, {typeof(T)} : {ex.Message}");
            }
        }

        public static Guid? GetGuidPref(string key)
        {
            var value = Preferences.Default.Get<string>(key, null);
            return value != null ? Guid.Parse(value) : null;
        }

        public static T GetPref<T>(string key, T defaultValue)
        {
            if (typeof(T).IsEnum)
            {
                var value = Preferences.Default.Get(key, defaultValue.ToString());
                return (T)Enum.Parse(typeof(T), value);
            }
            else if (typeof(T) == typeof(Guid?) || typeof(T) == typeof(Guid))
            {
                var guidValue = GetGuidPref(key);
                if (guidValue == null)
                {
                    return defaultValue;
                }
                return (T)Convert.ChangeType(guidValue, typeof(T));
            }
            else
            {
                return Preferences.Default.Get(key, defaultValue);
            }
        }

        public static async void SetSecure(string key, string value)
        {
            await SecureStorage.Default.SetAsync(key, value);
        }

        public static async Task<string> GetSecureAsync(string key)
        {
            return await SecureStorage.Default.GetAsync(key);
        }
    }
}