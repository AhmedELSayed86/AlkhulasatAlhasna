using System.Globalization;

namespace Alkhulasat.App.Converters
{
    public class CompletionColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isDone = (bool)value!;
            // إذا اكتمل الذكر نعطيه لون رمادي داكن، وإذا لم يكتمل نعطيه التركواز
            return isDone ? Color.FromArgb("#333333") : Color.FromArgb("#03DAC6");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

