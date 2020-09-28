using System;
using System.Globalization;
using System.Windows.Data;

namespace NepseApp.Converters
{
    public class PolarConverter : PolarConverter<int>
    {
        public PolarConverter() : base(1, 0, -1)
        {

        }
    }

    public class PolarConverter<T> : IValueConverter
    {
        public T PositiveValue { get; set; }
        public T NeutralValue { get; set; }
        public T NegativeValue { get; set; }

        public PolarConverter(T positiveValue, T neutralValue, T negativeValue)
        {
            PositiveValue = positiveValue;
            NeutralValue = neutralValue;
            NegativeValue = negativeValue;
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
            {
                if (number > 0) return PositiveValue;
                if (number < 0) return NegativeValue;
                return NeutralValue;
            }

            if (value is float floating)
            {
                if (floating > 0) return PositiveValue;
                if (floating < 0) return NegativeValue;
                return NeutralValue;
            }

            if (value is decimal decimalVal)
            {
                if (decimalVal > 0) return PositiveValue;
                if (decimalVal < 0) return NegativeValue;
                return NeutralValue;
            }

            if (value is long longVal)
            {
                if (longVal > 0) return PositiveValue;
                if (longVal < 0) return NegativeValue;
                return NeutralValue;
            }

            if (value is double doubleVal)
            {
                if (doubleVal > 0) return PositiveValue;
                if (doubleVal < 0) return NegativeValue;
                return NeutralValue;
            }

            return NeutralValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}