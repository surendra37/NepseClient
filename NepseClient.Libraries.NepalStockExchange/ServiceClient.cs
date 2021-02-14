using NepseClient.Commons.Utils;
using NepseClient.Libraries.NepalStockExchange.Responses;

using RestSharp;

using System;
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

        public GraphResponse[] GetGraphData(int id)
        {
            var api = new RestRequest($"/api/nots/market/graphdata/{id}");
            var response = Client.Get<GraphResponse[]>(api);
            return response.Data;
        }

        public SecurityDetailResponse GetSecurityDetail(int id)
        {
            var api = new RestRequest($"/api/nots/security/{id}"); // 132 NIB
            var response = Client.Get<SecurityDetailResponse>(api);
            return response.Data;
        }

        public CorporteActionResponse[] GetCorporateActions(int id)
        {
            var api = new RestRequest($"/api/nots/security/corporate-actions/{id}");
            var response = Client.Get<CorporteActionResponse[]>(api);
            return response.Data;
        }

        public SecurityStatResponse[] GetDailyTradStats()
        {
            var api = new RestRequest($"/api/nots/securityDailyTradeStat/58");
            var response = Client.Get<SecurityStatResponse[]>(api);
            return response.Data;
        }

        public NoticeReponse GetNotices(int? pageNumber = null)
        {
            var request = new RestRequest("/api/nots/news/notice/all");
            request.AddParameter("page", pageNumber);

            var response = Client.Get<NoticeReponse>(request);
            foreach (var item in response.Data.Content)
            {
                if (item.HasAttachment)
                {
                    //https://newweb.nepalstock.com.np/api/nots/news/notice/fetchFiles/8f0c90db48d40e37aac05936f9fa7708.pdf
                    item.NoticeFileFullPath = $"{Client.BaseUrl}api/nots/news/notice/fetchFiles/{item.NoticeFilePath}";
                }
            }
            return response.Data;
        }

        public FloorsheetResponse GetFloorsheet(int page = 0, string buyer = null, string seller = null, int size = 10, string sort = "contractId,desc")
        {
            var api = new RestRequest("/api/nots/nepse-data/floorsheet");
            api.AddParameter("size", size);
            api.AddParameter("sort", sort);
            api.AddParameter("page", page);
            if (!string.IsNullOrWhiteSpace(buyer))
            {
                api.AddParameter("buyerBroker", buyer);
            }
            if (!string.IsNullOrWhiteSpace(seller))
            {
                api.AddParameter("sellerBroker", seller);
            }

            var response = Client.Get<FloorsheetResponse>(api);
            return response.Data;
        }

        public LiveMarketResponse[] GetLiveMarket()
        {
            var api = new RestRequest("/api/nots/live-market");
            var response = Client.Get<LiveMarketResponse[]>(api);
            return response.Data;
        }

        public KeyValuePair<DateTime, double>[] GetNepseIndex()
        {
            var api = new RestRequest("/api/nots/graph/index/58");
            var response = Client.Get<double[][]>(api);
            return response.Data
                .Select(SelectGraphPredicate).ToArray();
        }
        public KeyValuePair<DateTime, double>[] GetSensitiveIndex()
        {
            var api = new RestRequest("/api/nots/graph/index/57");
            var response = Client.Get<double[][]>(api);
            return response.Data
                .Select(SelectGraphPredicate).ToArray();
        }

        private KeyValuePair<DateTime, double> SelectGraphPredicate(double[] args)
        {
            var date = new DateTime(1970, 1, 1).AddSeconds(args[0]).ToLocalTime();
            var value = args[1];
            return new KeyValuePair<DateTime, double>(date, value);
        }
    }
}
