using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WifViewer.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object True { get; set; }

        public object False { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return True;
            }
            else
            {
                return False;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == True;
        }
    }
}
