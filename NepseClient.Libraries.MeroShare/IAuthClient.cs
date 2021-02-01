
using RestSharp;

namespace NepseClient.Libraries.MeroShare
{
    public interface IAuthClient
    {
        RestClient Client { get; }
        void Authorize();
    }
}
