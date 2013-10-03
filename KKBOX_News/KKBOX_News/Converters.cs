using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KKBOX_News
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if ((String)parameter == "Xml")
            {
                return (Boolean)value ? Visibility.Collapsed : Visibility.Visible;
            }
            return (Boolean)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    public class BooleanToTextWarpConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            
            return (Boolean)value ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
