using MaterialDesignThemes.Wpf.Converters;
using System.Windows;

namespace NepseApp.Converters
{
    public class InvertBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertBooleanToVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible)
        { }
    }
}
