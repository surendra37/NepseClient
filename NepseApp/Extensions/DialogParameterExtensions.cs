
using Prism.Services.Dialogs;

namespace NepseApp.Extensions
{
    public static class DialogParameterExtensions
    {
        public static string GetTitle(this IDialogParameters value)
        {
            value.TryGetValue("Title", out string output);
            return output;
        }
        public static IDialogParameters AddTitle(this IDialogParameters value, string title)
        {
            value.Add("Title", title);
            return value;
        }

        public static string GetUsername(this IDialogParameters value)
        {
            value.TryGetValue("Username", out string output);
            return output;
        }

        public static IDialogParameters AddUsername(this IDialogParameters value, string username)
        {
            value.Add("Username", username);
            return value;
        }

        public static string GetPassword(this IDialogParameters value)
        {
            value.TryGetValue("Password", out string output);
            return output;
        }
        public static IDialogParameters AddPassword(this IDialogParameters value, string password)
        {
            value.Add("Password", password);
            return value;
        }

        public static bool GetRememberPassword(this IDialogParameters value)
        {
            value.TryGetValue("RememberPassword", out bool output);
            return output;
        }
        public static IDialogParameters AddRememberPassword(this IDialogParameters value, bool rememberPassword)
        {
            value.Add("RememberPassword", rememberPassword);
            return value;
        }
    }
}
