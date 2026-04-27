namespace Bharat.ToolKits.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool ValueAsBool(this IDictionary<string, object> dictionary, string key, bool defaultValue = false)
        {
            return dictionary.ContainsKey(key) && dictionary[key] is bool dictValue
                ? dictValue
                : defaultValue;
        }

        public static int ValueAsInt(this IDictionary<string, object> dictionary, string key, int defaultValue = 0)
        {
            return dictionary.ContainsKey(key) && dictionary[key] is int intValue
                ? intValue
                : defaultValue;
        }

#pragma warning disable CS8601 // Possible null reference assignment.
        public static T ValueAs<T>(this IDictionary<string, object> dictionary, string key, T defaultValue = default) => dictionary.ContainsKey(key) && dictionary[key] is T value
                ? value
                : defaultValue;
#pragma warning restore CS8601 // Possible null reference assignment.
    }
}