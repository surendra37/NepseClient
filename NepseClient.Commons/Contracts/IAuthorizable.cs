using RestSharp;

namespace NepseClient.Commons.Contracts
{
    public interface IAuthorizable
    {
        IRestClient Client { get; }
        bool IsAuthenticated { get; set;}
        void Authorize();
    }
}
