using System;
using RestSharp;

namespace TradeManagementSystemClient.Interfaces
{
    public interface IAuthorizable
    {
        void Authorize();
        IRestClient Client { get; }
    }
}
