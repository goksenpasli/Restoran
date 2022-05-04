using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restoran.Converter
{
    public sealed class RezervasyonSüreGeçtiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return Visibility.Visible;
            }
            return value is DateTime rezervasyontarihi
                ? rezervasyontarihi <= DateTime.Now ? Visibility.Collapsed : (object)Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}