using System.Globalization;
using System.Text.RegularExpressions;

namespace Bharat.ToolKits.Converters
{
    public class PascalCaseToWordsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = value as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return text ?? "";
            }

            // choose either simple or advanced logic here
            return Regex.Replace(text, "(?<!^)([A-Z])", " $1");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
