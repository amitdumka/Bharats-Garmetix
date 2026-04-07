using System.Globalization;

namespace Bharat.ToolKits.Converters
{
    public class StringContainsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string text && parameter is string containsText)
            {
                // Perform a case-insensitive check.
                return text.IndexOf(containsText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false; // Return false if inputs are not strings or invalid.
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // One-way conversion, not needed for this scenario.
        }
    }
}