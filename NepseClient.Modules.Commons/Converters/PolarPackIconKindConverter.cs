using MaterialDesignThemes.Wpf;

namespace NepseClient.Modules.Commons.Converters
{

    public class PolarPackIconKindConverter : PolarConverter<PackIconKind>
    {
        public PolarPackIconKindConverter() : base(PackIconKind.ArrowUp, PackIconKind.ArrowUpDown, PackIconKind.ArrowDown)
        {

        }
    }
}