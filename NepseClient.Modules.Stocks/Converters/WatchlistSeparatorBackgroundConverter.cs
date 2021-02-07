﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NepseClient.Modules.Stocks.Converters
{
    public class WatchlistSeparatorBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isWatching)
            {
                if (isWatching)
                    return Brushes.Yellow;
                else
                    return Brushes.Gray;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
