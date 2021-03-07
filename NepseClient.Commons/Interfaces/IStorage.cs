using System.Collections.Generic;
using System.Net;

namespace NepseClient.Commons.Interfaces
{
    public interface IStorage
    {
        CookieContainer Container { get; }
        IDictionary<string, string> LocalStorage { get; }void Save();
    }
}
