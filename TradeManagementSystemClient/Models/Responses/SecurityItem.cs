using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using System;

namespace TradeManagementSystemClient.Models.Responses
{
    public class SecurityItem : BaseSecurityItem, ISecurityItem
    {
        [JsonProperty("lastTradedTime")]
        public DateTime LastTradedDateTime { get; set; }
    }

    public class SecurityItem2 : BaseSecurityItem, ISecurityItem
    {
        [JsonProperty("lastTradedTime")]
        public int[] LastTradedTime { get; set; }

        public DateTime LastTradedDateTime =>
            new DateTime(LastTradedTime[0], LastTradedTime[1], LastTradedTime[2],
                LastTradedTime[3], LastTradedTime[4], LastTradedTime[5], DateTimeKind.Utc);
    }
    public class BaseSecurityItem
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("ltp")]
        public float LastTradedPrice { get; set; }

        [JsonProperty("percentChange")]
        public float PercentChange { get; set; }

        [JsonProperty("high")]
        public float High { get; set; }

        [JsonProperty("low")]
        public float Low { get; set; }

        [JsonProperty("open")]
        public float Open { get; set; }

        [JsonProperty("lastTradedVolume")]
        public int LastTradedVolume { get; set; }

        [JsonProperty("change")]
        public float Change { get; set; }

        [JsonProperty("previousClose")]
        public float PreviousClose { get; set; }

        public int PolarChange
        {
            get
            {
                if (Change > 0) return 1;
                if (Change < 0) return -1;
                return 0;
            }
        }
    }
}