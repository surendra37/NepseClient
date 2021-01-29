using NepseClient.Modules.Commons.Interfaces;

namespace NepseClient.Modules.Commons.Extensions
{
    public static class ApplicationCommandExtensions
    {
        public static void ShowMessage(this IApplicationCommand appCommand, string message)
        {
            appCommand.ShowMessage?.Invoke(message);
        }

        public static void HideMessage(this IApplicationCommand appCommand)
        {
            appCommand.ShowMessage?.Invoke(string.Empty);
        }
    }
}
