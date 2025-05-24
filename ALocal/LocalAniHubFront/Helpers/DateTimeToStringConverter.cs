using System.Globalization;
using System.Windows.Data;

namespace LocalAniHubFront.Helpers
{
    public class DateTimeToStringConverter : IValueConverter
    {
        // DateTime -> string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy.M.d HH:mm", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        // string -> DateTime
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParseExact(value?.ToString(), "yyyy.M.d HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
