namespace Bharat.ToolKits.Extensions;

/// <summary>
/// Enum Extensions
///     Convert enum to values and values to enum.
/// </summary>
public static class EnumExtensions
{
    public class EnumValue
    {
        public string Name { get; set; } = "NotFound";
        public int Value { get; set; } = 0;
    }

    public static List<EnumValue> GetValues<T>() where T : Enum
    {
        List<EnumValue> values = [];
        foreach (var itemType in Enum.GetValues(typeof(T)))
        {
            //For each value of this enumeration, add a new EnumValue instance
            values.Add(new EnumValue()
            {
                Name = Enum.GetName(typeof(T), itemType) ?? "Not Found",
                Value = (int)itemType
            });
        }
        return values;
    }

    public static string ToReadableString(this Enum value)
    {
        return value.ToString().Replace("_", " ");
    }

    public static T ToEnum<T>(this string value) where T : struct, Enum
    {
        return Enum.TryParse<T>(value, out var result) ? result : default;
    }


}




