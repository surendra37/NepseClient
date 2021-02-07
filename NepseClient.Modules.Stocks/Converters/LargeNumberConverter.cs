using NepseClient.Modules.Stocks.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NepseClient.Modules.Stocks.Converters
{

    public class LargeNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double largeNumber)
            {
                return NumberUtils.GetMarketCapText(largeNumber);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}