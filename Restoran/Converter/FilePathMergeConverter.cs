using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Restoran.ViewModel;

namespace Restoran.Converter
{
    public sealed class FilePathMergeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !DesignerProperties.GetIsInDesignMode(new DependencyObject()) && value is string filename && !string.IsNullOrEmpty(filename) && File.Exists($"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}")
                ? $"{Path.GetDirectoryName(MainViewModel.xmldatapath)}\\{filename}"
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as BitmapSource;
        }
    }
}