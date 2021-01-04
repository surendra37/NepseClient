using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

using RestSharp;

namespace NepseService.TradeManagementSystem.Utils
{

    public static class CookieUtils
    {
        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(file))
            {
                try
                {
                    Log.Debug("Writing cookies to disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                    Log.Debug("Done.");
                }
                catch (Exception e)
                {
                    Log.Debug("Problem writing cookies to disk: " + e.GetType());
                }
            }
        }

        public static CookieContainer ReadCookiesFromDisk(string file)
        {

            try
            {
                if (!File.Exists(file)) return new CookieContainer();

                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    Log.Debug("Reading cookies from disk... ");
                    BinaryFormatter formatter = new BinaryFormatter();
                    Log.Debug("Done.");
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Log.Debug("Problem reading cookies from disk: " + e.GetType());
                return new CookieContainer();
            }
        }

        public static Cookie GetCookieFromValues(string[] parts)
        {
            var output = new Cookie();
            foreach (var part in parts)
            {
                var pair = part.Split('=');
                var key = pair.ElementAtOrDefault(0);
                var value = pair.ElementAtOrDefault(1);
                if (key.Equals("Version"))
                {
                    output.Version = int.Parse(value);
                }
                else if (key.Equals("Comment"))
                {
                    output.Comment = value;
                }
                else if (key.Equals("Domain"))
                {
                    output.Domain = value;
                }
                else if (key.Equals("Path"))
                {
                    output.Path = value;
                }
                else if (key.Equals("Max-Age"))
                {
                    output.Expires = DateTime.Now.AddSeconds(int.Parse(value));
                }
                else if (key.Equals("Secure"))
                {
                    output.Secure = true;
                }
                else if (key.Equals("HttpOnly"))
                {
                    output.HttpOnly = true;
                }
                else
                {
                    output.Name = key;
                    output.Value = value;
                }

            }

            return output;
        }

        public static void ParseCookies(IRestResponse response, CookieContainer cookieJar, Uri uri)
        {
            var cookieHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("Set-Cookie"));
            if (cookieHeader is null) return;

            var header = cookieHeader.Value.ToString()
                .Replace("Domain=;", string.Empty);

            try
            {
                cookieJar.SetCookies(uri, header);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to set cookies");
            }
        }
    }
}