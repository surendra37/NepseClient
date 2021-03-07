using NepseClient.Commons.Constants;
using NepseClient.Commons.Interfaces;
using NepseClient.Commons.Utils;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace NepseApp.Models
{
    public class Storage : IStorage
    {
        private readonly string _cookieLocation;
        private readonly string _localStorageLocation;
        public CookieContainer Container { get; }
        public IDictionary<string, string> LocalStorage { get; }

        public Storage()
        {
            _cookieLocation = Path.Combine(PathConstants.AppDataPath.Value, "cookies.dat");
            Container = CookieUtils.ReadCookiesFromDisk(_cookieLocation);

            _localStorageLocation = Path.Combine(PathConstants.AppDataPath.Value, "local-storage.dat");
            var storageValue = string.Empty;
            if (File.Exists(_localStorageLocation))
            {
                storageValue = File.ReadAllText(_localStorageLocation);
            }
            LocalStorage = JsonConvert.DeserializeObject<Dictionary<string, string>>(storageValue) ?? new Dictionary<string, string>();
        }

        public void Save()
        {
            CookieUtils.WriteCookiesToDisk(_cookieLocation, Container);
            File.WriteAllText(_localStorageLocation, JsonConvert.SerializeObject(LocalStorage));
        }
    }
}
