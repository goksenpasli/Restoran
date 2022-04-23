using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Restoran.Converter
{
    public sealed class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue
                ? double.Parse(values[0].ToString()) * int.Parse(values[1].ToString())
                : (object)0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}