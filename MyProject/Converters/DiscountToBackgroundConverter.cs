using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyProject.Converters
{
    public class DiscountToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double discount && discount > 15)
                return new SolidColorBrush(Color.FromRgb(255, 220, 220));

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}