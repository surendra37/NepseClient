using System.IO;

using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using NepseClient.Commons.Utils;

using Newtonsoft.Json;

namespace NepseApp.Models
{
    public class TmsConfigurationFile
    {
        private static readonly string _filePath = Path.Combine(Constants.AppDataPath.Value, "tms-config.json");
        public string Password { get; set; }
        public string Username { get; set; }
        public bool RememberPassword { get; set; }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filePath, json);
            Settings.Default.Save();
        }

        public static TmsConfigurationFile LoadFromFile()
        {
            if (File.Exists(_filePath))
            {

                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<TmsConfigurationFile>(json);
            }
            else
            {
                return new TmsConfigurationFile();
            }
        }
    }

    public class TmsConfiguration : ITmsConfiguration
    {
        private readonly TmsConfigurationFile _config;
        public string BaseUrl
        {
            get => Settings.Default.TmsBaseUrl;
            set => Settings.Default.TmsBaseUrl = value;
        }
        public string Username
        {
            get => _config.Username;
            set => _config.Username = value;
        }
        public string Password
        {
            get => StringCipher.Decrypt(_config.Password);
            set => _config.Password = StringCipher.Encrypt(value);
        }
        public bool RememberPassword
        {
            get => _config.RememberPassword;
            set => _config.RememberPassword = value;
        }

        public TmsConfiguration()
        {
            _config = TmsConfigurationFile.LoadFromFile();
        }

        public void Save()
        {
            _config.Save();
            Settings.Default.Save();
        }
    }
}
