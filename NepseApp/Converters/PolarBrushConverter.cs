using System.Windows.Media;

namespace NepseApp.Converters
{

    public class PolarBrushConverter : PolarConverter<Brush>
    {
        public PolarBrushConverter() : base(Brushes.Green, Brushes.Black, Brushes.Red)
        {

        }
    }
}