using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LocalAniHubFront.Helpers
{
    public class ScoreToStyleConverter : IValueConverter
    {
        public Style StyleIfLessThanThreshold { get; set; }
        public Style StyleIfGreaterOrEqualThreshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int score && parameter is string thresholdStr && int.TryParse(thresholdStr, out int threshold))
            {
                return score < threshold ? StyleIfLessThanThreshold : StyleIfGreaterOrEqualThreshold;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
