using NepseApp.Utils;

using NepseClient.Commons.Contracts;

namespace NepseApp.Models
{

    public class TmsConfiguration : ITmsConfiguration
    {
        public string BaseUrl
        {
            get => Settings.Default.TmsHost;
            set => Settings.Default.TmsHost = value;
        }
        public string Username
        {
            get => Settings.Default.TmsUsername;
            set => Settings.Default.TmsUsername = value;
        }
        public string Password
        {
            get => StringCipher.Decrypt(Settings.Default.TmsPassword);
            set => Settings.Default.TmsPassword = StringCipher.Encrypt(value);
        }
        public bool RememberPassword
        {
            get => Settings.Default.TmsRememberPassword;
            set => Settings.Default.TmsRememberPassword = value;
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}