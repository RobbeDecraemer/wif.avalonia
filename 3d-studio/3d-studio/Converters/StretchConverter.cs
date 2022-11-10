using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace WifViewer.Converters
{
    public class StretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Stretch.Uniform;
            }
            else
            {
                return Stretch.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
