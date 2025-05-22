using System;
using System.Globalization;
using System.Windows.Data;

namespace LocalAniHubFront.Helpers
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "00:00";
            if (!long.TryParse(value.ToString(), out long ms)) return "00:00";
            var ts = TimeSpan.FromMilliseconds(ms);
            return $"{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
