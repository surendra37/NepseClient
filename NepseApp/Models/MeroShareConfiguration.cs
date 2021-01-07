using NepseClient.Commons.Contracts;
using NepseClient.Commons.Utils;

namespace NepseApp.Models
{

    public class MeroShareConfiguration : IMeroShareConfiguration
    {
        public string BaseUrl
        {
            get => Settings.Default.MeroShareBaseUrl;
            set => Settings.Default.MeroShareBaseUrl = value;
        }
        public string Username
        {
            get => Settings.Default.MeroshareUsername;
            set => Settings.Default.MeroshareUsername = value;
        }
        public string Password
        {
            get => StringCipher.Decrypt(Settings.Default.MerosharePassword);
            set => Settings.Default.MerosharePassword = StringCipher.Encrypt(value);
        }
        public string ClientId
        {
            get => Settings.Default.MeroshareClientId;
            set => Settings.Default.MeroshareClientId = value;
        }
        public string[] Demat
        {
            get => Settings.Default.MeroshareDemat.Split(',');
            set => Settings.Default.MeroshareDemat = string.Join(',', value);
        }

        public bool RememberPassword
        {
            get => Settings.Default.MeroshareRememberPassword;
            set => Settings.Default.MeroshareRememberPassword = value;
        }

        public string AuthHeader
        {
            get => StringCipher.Decrypt(Settings.Default.MeroShareAuthorization);
            set => Settings.Default.MeroShareAuthorization = StringCipher.Encrypt(value);
        }

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}