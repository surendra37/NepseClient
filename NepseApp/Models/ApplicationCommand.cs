using MaterialDesignThemes.Wpf;
using Prism.Commands;

namespace NepseApp.Models
{
    public class ApplicationCommand : IApplicationCommand
    {
        public CompositeCommand RefreshCommand { get; } = new CompositeCommand(true);
        public ISnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();
        public CompositeCommand SnackbarMessageQueueCommand { get; } = new CompositeCommand();
    }
}
