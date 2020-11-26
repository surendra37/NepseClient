
using MaterialDesignThemes.Wpf;

using Prism.Mvvm;

namespace NepseApp.Models
{

    public class SettingsItem : BindableBase
    {
        public string Name { get; set; }
        public PackIconKind IconKind { get; set; }

        public virtual void Save() { }
        public virtual void Reset() { }
    }
}
