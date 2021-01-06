
using RestSharp;

namespace TradeManagementSystemClient
{
    public interface IAuthClient
    {
        RestClient Client { get; }
        void Authorize();
    }
}
