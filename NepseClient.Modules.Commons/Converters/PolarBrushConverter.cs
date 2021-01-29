using System.Windows.Media;

namespace NepseClient.Modules.Commons.Converters
{

    public class PolarBrushConverter : PolarConverter<Brush>
    {
        public PolarBrushConverter() : base(Brushes.Green, Brushes.Black, Brushes.Red)
        {

        }
    }
}