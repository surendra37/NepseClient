using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NepseClient.Modules.Commons.Converters
{
    public class MeroShareAsbaVisibility : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var link = value?.ToString() ?? string.Empty;

            var visible = link.Equals("apply") || link.Equals("edit");

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}