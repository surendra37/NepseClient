using NepseApp.Utils;
using Serilog;
using System.Collections.Generic;
using System.Configuration;

namespace NepseClient.Commons
{
    public static class ConfigUtils
    {
        public static string TmsHost { get; set; }
        public static string TmsUsername { get; set; }
        public static string TmsPassword { get; set; }

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
            TmsHost = ConfigurationManager.AppSettings["tms_host"];
            TmsUsername = ConfigurationManager.AppSettings["tms_username"];
            TmsPassword = StringCipher.Decrypt(ConfigurationManager.AppSettings["tms_password"]);
            bool.TryParse(ConfigurationManager.AppSettings["remember_password"], out bool rememberPassword);
            bool.TryParse(ConfigurationManager.AppSettings["auto_login"], out bool autoLogin);

            RememberPassword = rememberPassword;
            AutoLogin = autoLogin && rememberPassword;
        }

        public static void SaveSettings()
        {
            AddOrUpdateAppSettings(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("tms_host", TmsHost),
                new KeyValuePair<string, string>("tms_username", TmsUsername),
                new KeyValuePair<string, string>("remember_password", RememberPassword.ToString()),
                new KeyValuePair<string, string>("auto_login", AutoLogin.ToString()),
                new KeyValuePair<string, string>("tms_password", StringCipher.Encrypt(TmsPassword)),
            });
        }

        public static void ResetSettings()
        {
            TmsHost = TmsUsername = TmsPassword = string.Empty;
            RememberPassword = AutoLogin = false;
            SaveSettings();
        }
    }
}
