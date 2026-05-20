using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MyProject.Converters
{
    public class BookImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\covers", strValue);

            return "Not found";
        }

        // Из UI → обратно в источник (если нужен TwoWay binding)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
                return str;

            return false;
        }
    }
}
