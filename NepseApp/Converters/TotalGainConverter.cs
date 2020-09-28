using NepseClient.Commons.Contracts;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NepseApp.Converters
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

    public class TotalGainBrushConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IScripResponse scrip)
            {
                var output = scrip.LTPTotal - (scrip.WaccValue * scrip.CurrentBalance);
                if(output > 0) return Brushes.Green;
                if(output < 0) return Brushes.Red;
                return Brushes.Black;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DailyGainBrushConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IScripResponse scrip)
            {
                var output = scrip.LTPTotal - scrip.PreviousTotal;
                if(output > 0) return Brushes.Green;
                if(output < 0) return Brushes.Red;
                return Brushes.Black;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}