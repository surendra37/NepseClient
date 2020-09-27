using MaterialDesignThemes.Wpf;
using Prism.Commands;

namespace NepseApp.Models
{
    public interface IApplicationCommand
    {
        CompositeCommand RefreshCommand { get; }
        ISnackbarMessageQueue MessageQueue { get; }
    }
}