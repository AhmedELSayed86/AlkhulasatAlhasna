using System.Globalization;

namespace Alkhulasat.App.Converters
{
    // في مجلد Converters
    public class CategoryToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // إذا كان التصنيف هو 'Children' فاجعله مخفياً (False)
            return value?.ToString() != "Children";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
    }

}
