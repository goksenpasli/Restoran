using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Restoran.Model;

namespace Restoran.Converter
{
    public sealed class SiparişSumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject())
                ? string.Empty
                : value is ObservableCollection<Sipariş> Siparişler ? Siparişler.SiparişToplamları() : (object)string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}