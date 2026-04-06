using System.Globalization;

namespace Bharat.ToolKits.Converters
{

    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        // Remember to add the IsNotNullOrEmptyConverter to your App.xaml resources:
        // <converters:IsNotNullOrEmptyConverter x:Key="IsNotNullOrEmptyConverter" />
        // And ensure the namespace MauiOnboardingApp.Converters exists and the file is in a Converters folder.
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return !string.IsNullOrEmpty(str);
            }
            if (value is ICollection<string> collection) // For CommunityToolkit.Mvvm errors
            {
                return collection != null && collection.Count > 0;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
