using NepseApp.Utils;
using NepseApp.Views;
using Serilog;
using System.Collections.Generic;
using System.Configuration;

namespace NepseClient.Commons
{
    public static class ConfigUtils
    {
        public static string SelectedTab { get; set; }

        public static string TmsHost { get; set; }
        public static string TmsUsername { get; set; }
        public static string TmsPassword { get; set; }

        public static int MeroshareClientId { get; set; }
        public static string MeroshareUsername { get; set; }
        public static string MerosharePassword { get; set; }
        public static bool MeroshareRememberPassword { get; set; }

        public static bool RememberPassword { get; set; }
        public static bool AutoLogin { get; set; }

        public static void AddOrUpdateAppSettings(params KeyValuePair<string, string>[] pairs)
        {
            AddOrUpdateAppSettings((IEnumerable<KeyValuePair<string, string>>)pairs);
        }
        public static void AddOrUpdateAppSettings(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                foreach (var pair in pairs)
                {
                    var key = pair.Key;
                    var value = pair.Value;
                    if (settings[key] == null)
                    {
                        settings.Add(key, value);
                    }
                    else
                    {
                        settings[key].Value = value;
                    }
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException exception)
            {
                Log.Error(exception, "Error writing app settings");
            }
        }

        public static void LoadSettings()
        {
            SelectedTab = ConfigurationManager.AppSettings["selected_tab"] ?? nameof(DashboardPage);
            TmsHost = ConfigurationManager.AppSettings["tms_host"];
            TmsUsername = ConfigurationManager.AppSettings["tms_username"];
            MeroshareUsername = ConfigurationManager.AppSettings["meroshare_username"];

            TmsPassword = StringCipher.Decrypt(ConfigurationManager.AppSettings["tms_password"]);
            MerosharePassword = StringCipher.Decrypt(ConfigurationManager.AppSettings["meroshare_password"]);

            int.TryParse(ConfigurationManager.AppSettings["meroshare_client_id"], out int meroshareClientId);

            bool.TryParse(ConfigurationManager.AppSettings["remember_password"], out bool rememberPassword);
            bool.TryParse(ConfigurationManager.AppSettings["auto_login"], out bool autoLogin);
            bool.TryParse(ConfigurationManager.AppSettings["meroshare_remember_password"], out bool meroshareRememberPassword);

            RememberPassword = rememberPassword;
            AutoLogin = autoLogin && rememberPassword;

            MeroshareClientId = meroshareClientId;
            MeroshareRememberPassword = meroshareRememberPassword;
        }

        public static void SaveSettings()
        {
            AddOrUpdateAppSettings(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("selected_tab", SelectedTab),
                new KeyValuePair<string, string>("tms_host", TmsHost),
                new KeyValuePair<string, string>("tms_username", TmsUsername),
                new KeyValuePair<string, string>("remember_password", RememberPassword.ToString()),
                new KeyValuePair<string, string>("auto_login", AutoLogin.ToString()),
                new KeyValuePair<string, string>("tms_password", StringCipher.Encrypt(TmsPassword)),

                new KeyValuePair<string, string>("meroshare_client_id", MeroshareClientId.ToString()),
                new KeyValuePair<string, string>("meroshare_username", MeroshareUsername),
                new KeyValuePair<string, string>("meroshare_password", StringCipher.Encrypt(MerosharePassword)),
                new KeyValuePair<string, string>("meroshare_remember_password", MerosharePassword.ToString()),
            });
        }

        public static void ResetSettings()
        {
            SelectedTab = nameof(DashboardPage);
            TmsHost = TmsUsername = TmsPassword = string.Empty;
            RememberPassword = AutoLogin = false;

            MeroshareClientId = 0;
            MeroshareUsername = MerosharePassword = string.Empty;
            MeroshareRememberPassword = false;

            SaveSettings();
        }
    }
}
