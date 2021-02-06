using NepseClient.Commons.Utils;
using NepseClient.Libraries.NepalStockExchange.Responses;

using RestSharp;

using System.Collections.Generic;
using System.Linq;

namespace NepseClient.Libraries.NepalStockExchange
{
    public class ServiceClient
    {
        public IRestClient Client { get; init; }

        public ServiceClient()
        {
            Client = RestClientUtils.CreateNewClient("https://newweb.nepalstock.com");
        }

        public PageableResponse<TodayPriceContent> GetTodaysPrice()
        {
            //page=2&size=20&businessDate=2021-02-04&sort=highPrice,asc
            var api = new RestRequest("/api/nots/nepse-data/today-price");
            var response = Client.Get<PageableResponse<TodayPriceContent>>(api);
            return response.Data;
        }

        public IList<TodayPriceContent> GetTodaysPriceAll()
        {
            var output = new List<TodayPriceContent>();
            //page=2&size=20&businessDate=2021-02-04&sort=highPrice,asc
            var api = new RestRequest("/api/nots/nepse-data/today-price");
            var hasNextPage = true;
            while (hasNextPage)
            {
                var response = Client.Get<PageableResponse<TodayPriceContent>>(api);
                var data = response.Data;
                output.AddRange(data.Content);
                var currentPage = data.Pageable.PageNumber + 1;
                hasNextPage = currentPage < response.Data.TotalPages;
                if (hasNextPage)
                {
                    api = new RestRequest("/api/nots/nepse-data/today-price");
                    api.AddParameter("page", currentPage); // here counting of current page starts from 0 so current page is next page
                    api.AddParameter("size", data.Size);
                    api.AddParameter("businessDate", data.Content.First().BusinessDate);
                }
            }
            return output;
        }

        public MarketStatusResponse GetMarketStatus()
        {
            var request = new RestRequest("/api/nots/nepse-data/market-open");
            var response = Client.Get<MarketStatusResponse>(request);
            return response.Data;
        }

        public NewsAndAlertResponse[] GetNews()
        {
            var api = new RestRequest("/api/nots/news/media/news-and-alerts");
            var response = Client.Get<NewsAndAlertResponse[]>(api);
            return response.Data;
        }
    }
}
