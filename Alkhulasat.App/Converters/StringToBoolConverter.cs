using System.Globalization;

namespace Alkhulasat.App.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // إذا كان النص غير فارغ يعيد true (ليظهر الليبل)
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

