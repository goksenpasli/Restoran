using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restoran.Converter
{
    public sealed class BooltoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? Visibility.Visible
                : (object)(parameter != null
                ? (bool)value ? Visibility.Collapsed : Visibility.Visible
                : value != null && (bool)value ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}