using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TradeManagementSystemClient.Models.Responses
{

    public class StockQuoteResponse : IStockQuoteResponse
    {
        [JsonProperty("security")]
        [JsonConverter(typeof(ConcreteConverter<SecurityResponse>))]
        public ISecurityResponse Security { get; set; }

        [JsonProperty("change")]
        public double Change { get; set; }

        [JsonProperty("changePercentage")]
        public double ChangePercentage { get; set; }

        [JsonProperty("ltp")]
        public double Ltp { get; set; }

        [JsonProperty("averageTradedPrice")]
        public double AverageTradedPrice { get; set; }

        [JsonProperty("openPrice")]
        public double OpenPrice { get; set; }

        [JsonProperty("dayHigh")]
        public double DayHigh { get; set; }

        [JsonProperty("dayLow")]
        public double DayLow { get; set; }

        [JsonProperty("closePrice")]
        public double ClosePrice { get; set; }

        [JsonProperty("lastTradedQty")]
        public int LastTradedQty { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("lastTradedTime")]
        public string LastTradedTime { get; set; }

        [JsonProperty("totalBuyQty")]
        public int TotalBuyQty { get; set; }

        [JsonProperty("totalSellQty")]
        public int TotalSellQty { get; set; }

        [JsonProperty("topBuy")]
        [JsonConverter(typeof(ConcreteArraryConverter<TopOrderResponse>))]
        public IEnumerable<ITopOrderResponse> TopBuy { get; set; }

        [JsonProperty("topSell")]
        [JsonConverter(typeof(ConcreteArraryConverter<TopOrderResponse>))]
        public IEnumerable<ITopOrderResponse> TopSell { get; set; }
    }
}