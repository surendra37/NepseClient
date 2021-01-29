﻿using NepseClient.Commons.Contracts;
using System;
using System.Globalization;
using System.Windows.Data;

namespace NepseClient.Modules.Commons.Converters
{

    public class TotalGainConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IScripResponse scrip)
            {
                return scrip.LTPTotal - (scrip.WaccValue * scrip.CurrentBalance);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}