using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LocalAniHubFront.Helpers
{
    public class DesaturateBrushConverter : IValueConverter
    {
        public double DesaturationAmount { get; set; } = 0.2; // 越高越灰

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidBrush)
            {
                Color original = solidBrush.Color;

                // 与灰色混合（#808080）
                byte grayR = 128;
                byte grayG = 128;
                byte grayB = 128;

                byte r = (byte)(original.R * (1 - DesaturationAmount) + grayR * DesaturationAmount);
                byte g = (byte)(original.G * (1 - DesaturationAmount) + grayG * DesaturationAmount);
                byte b = (byte)(original.B * (1 - DesaturationAmount) + grayB * DesaturationAmount);

                return new SolidColorBrush(Color.FromArgb(original.A, r, g, b));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
