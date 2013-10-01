﻿using System;
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
            return (Boolean)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}