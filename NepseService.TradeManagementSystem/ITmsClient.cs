using System;

using NepseService.TradeManagementSystem.Models.Responses;

using RestSharp;

namespace NepseService.TradeManagementSystem
{
    public interface ITmsClient
    {
        Action<TmsClient> Authorize { get; set; }
        IRestClient Client { get; }
        string CookiePath { get; init; }
        bool UseCookies { get; init; }

        string GetBusinessDate();
        ScripReponse[] GetMyPortfolio(string clientId, bool retry = true);
        void SignIn(string username, string password);
        void SignOut();
    }
}