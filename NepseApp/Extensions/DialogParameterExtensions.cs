
using Prism.Services.Dialogs;

using TradeManagementSystemClient.Models.Responses.MeroShare;

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

        public static IDialogParameters AddShareReport(this IDialogParameters value, AsbaShareReportDetailResponse input)
        {
            value.Add("ShareReport", input);
            return value;
        }
        public static AsbaShareReportDetailResponse GetShareReport(this IDialogParameters value)
        {
            return value.GetValue<AsbaShareReportDetailResponse>("ShareReport");
        }

        public static IDialogParameters AddApplicantFormDetail(this IDialogParameters value, ApplicantFormReportDetail input)
        {
            value.Add("FormDetail", input);
            return value;
        }
        public static ApplicantFormReportDetail GetFormDetail(this IDialogParameters value)
        {
            return value.GetValue<ApplicantFormReportDetail>("FormDetail");
        }
    }
}
