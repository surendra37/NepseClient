﻿using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
namespace TradeManagementSystemClient.Models.Responses
{
    public class ScripResponse 
    {
        [JsonProperty("cdsFreeBalance")]
        public float FreeBalance { get; set; }

        [JsonProperty("cdsTotalBalance")]
        public float TotalBalance { get; set; }

        [JsonProperty("currentBalance")]
        public float CurrentBalance { get; set; }

        [JsonProperty("ltp")]
        public float LastTransactionPrice { get; set; }

        public float PreviousClosePrice { get; set; }

        public string Scrip { get; set; }

        [JsonProperty("symbolName")]
        public string Name { get; set; }

        [JsonProperty("valueAsOfLTP")]
        public float LTPTotal { get; set; }

        [JsonProperty("valueAsOfPreviousClosePrice")]
        public float PreviousTotal { get; set; }

        public float WaccValue { get; set; }

        public float DailyGain => (LastTransactionPrice - PreviousClosePrice) * TotalBalance;
        public float TotalGain => (LastTransactionPrice - WaccValue) * TotalBalance;

        public ScripResponse()
        {
        }
    }
}
