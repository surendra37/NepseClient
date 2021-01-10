using RestSharp;

namespace TradeManagementSystemClient.Interfaces
{
    public interface IAuthorizable
    {
        bool IsAuthenticated { get; set; }
        void Authorize();
        IRestClient Client { get; }
    }
}
