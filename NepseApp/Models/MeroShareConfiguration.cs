using System.IO;

using NepseClient.Commons.Contracts;
using NepseClient.Commons.Utils;

using Newtonsoft.Json;

namespace NepseApp.Models
{
    public class MeroShareConfigurationFile
    {
        private const string _filePath = "meroshare-config.json";
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Demat { get; set; }
        public bool RememberPassword { get; set; }
        public string AuthHeader { get; set; }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filePath, json);
            Settings.Default.Save();
        }

        public static MeroShareConfigurationFile NewInstance()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<MeroShareConfigurationFile>(json);
            }
            else
            {
                return new MeroShareConfigurationFile();
            }
        }
    }
    public class MeroShareConfiguration : IMeroShareConfiguration
    {
        private readonly MeroShareConfigurationFile _config;
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
        public string ClientId
        {
            get => Settings.Default.MeroShareClientId;
            set => Settings.Default.MeroShareClientId = value;
        }
        public string[] Demat
        {
            get => _config.Demat;
            set => _config.Demat = value;
        }
        public bool RememberPassword
        {
            get => _config.RememberPassword;
            set => _config.RememberPassword = value;
        }
        public string AuthHeader
        {
            get => StringCipher.Decrypt(_config.AuthHeader);
            set => _config.AuthHeader = StringCipher.Encrypt(value);
        }

        public MeroShareConfiguration()
        {
            _config = MeroShareConfigurationFile.NewInstance();
        }

        public void Save()
        {
            _config.Save();
            Settings.Default.Save();
        }
    }
}