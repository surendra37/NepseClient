using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class CachedDataResponse : ICachedDataResponse
    {
        [JsonProperty("operatingParameter")]
        [JsonConverter(typeof(ConcreteArraryConverter<OperatingParameter>))]
        public IOperatingParameter[] OperatingParameter { get; set; }

        [JsonProperty("productType")]
        [JsonConverter(typeof(ConcreteArraryConverter<ProductType>))]
        public IProductType[] ProductType { get; set; }

        [JsonProperty("orderType")]
        [JsonConverter(typeof(ConcreteArraryConverter<OrderType>))]
        public IOrderType[] OrderType { get; set; }

        [JsonProperty("security")]
        [JsonConverter(typeof(ConcreteArraryConverter<SecurityResponse>))]
        public ISecurityResponse[] Security { get; set; }

        [JsonProperty("dealerSecurity")]
        public object[] DealerSecurity { get; set; }

        [JsonProperty("validity")]
        [JsonConverter(typeof(ConcreteArraryConverter<Validity>))]
        public IValidity[] Validity { get; set; }

        [JsonProperty("instrumentType")]
        [JsonConverter(typeof(ConcreteArraryConverter<InstrumentType>))]
        public IInstrumentType[] InstrumentType { get; set; }

        [JsonProperty("currentMarketSession")]
        [JsonConverter(typeof(ConcreteConverter<CurrentMarketSession>))]
        public ICurrentMarketSession CurrentMarketSession { get; set; }

        [JsonProperty("marketType")]
        [JsonConverter(typeof(ConcreteArraryConverter<MarketType>))]
        public IMarketType[] MarketType { get; set; }

        [JsonProperty("postCloseSecurities")]
        public object[] PostCloseSecurities { get; set; }
    }
}