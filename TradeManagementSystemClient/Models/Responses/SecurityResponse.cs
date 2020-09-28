using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    public class SecurityResponse : ISecurityResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("securityName")]
        public string SecurityName { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("fiftyTwoWeekhigh")]
        public double FiftyTwoWeekhigh { get; set; }

        [JsonProperty("fiftyTwoWeekLow")]
        public double FiftyTwoWeekLow { get; set; }
    }

    //public class SecurityResponse : ISecurityResponse
    //{
    //    [JsonProperty("id")]
    //    public int Id { get; set; }

    //    [JsonProperty("symbol")]
    //    public string Symbol { get; set; }

    //    [JsonProperty("isin")]
    //    public string Isin { get; set; }

    //    [JsonProperty("permittedToTrade")]
    //    public string PermittedToTrade { get; set; }

    //    [JsonProperty("listingDate")]
    //    public string ListingDate { get; set; }

    //    [JsonProperty("creditRating")]
    //    public string CreditRating { get; set; }

    //    [JsonProperty("tickSize")]
    //    public double TickSize { get; set; }

    //    [JsonProperty("instrumentType")]
    //    public InstrumentType InstrumentType { get; set; }

    //    [JsonProperty("exchange")]
    //    public Exchange Exchange { get; set; }

    //    [JsonProperty("exchangeSecurityId")]
    //    public int ExchangeSecurityId { get; set; }

    //    [JsonProperty("boardLotQuantity")]
    //    public int BoardLotQuantity { get; set; }

    //    [JsonProperty("issuePrice")]
    //    public double IssuePrice { get; set; }

    //    [JsonProperty("issuedCapital")]
    //    public double IssuedCapital { get; set; }

    //    [JsonProperty("bookClosureStartDate")]
    //    public object BookClosureStartDate { get; set; }

    //    [JsonProperty("bookClosureEndDate")]
    //    public object BookClosureEndDate { get; set; }

    //    [JsonProperty("recordDate")]
    //    public object RecordDate { get; set; }

    //    [JsonProperty("suspensionDate")]
    //    public object SuspensionDate { get; set; }

    //    [JsonProperty("noDeliveryStartDate")]
    //    public object NoDeliveryStartDate { get; set; }

    //    [JsonProperty("noDeliveryEndDate")]
    //    public object NoDeliveryEndDate { get; set; }

    //    [JsonProperty("marketProtectionPercentage")]
    //    public double MarketProtectionPercentage { get; set; }

    //    [JsonProperty("exDate")]
    //    public object ExDate { get; set; }

    //    [JsonProperty("revocationDate")]
    //    public object RevocationDate { get; set; }

    //    [JsonProperty("priceDenominator")]
    //    public double PriceDenominator { get; set; }

    //    [JsonProperty("priceNumerator")]
    //    public double PriceNumerator { get; set; }

    //    [JsonProperty("companyName")]
    //    public string CompanyName { get; set; }

    //    [JsonProperty("dprRangeHigh")]
    //    public double DprRangeHigh { get; set; }

    //    [JsonProperty("dprRangeLow")]
    //    public double DprRangeLow { get; set; }

    //    [JsonProperty("fiftyTwoWeekhigh")]
    //    public double FiftyTwoWeekhigh { get; set; }

    //    [JsonProperty("fiftyTwoWeekLow")]
    //    public double FiftyTwoWeekLow { get; set; }

    //    [JsonProperty("remarks")]
    //    public string Remarks { get; set; }

    //    [JsonProperty("maxAllowedQuantity")]
    //    public int MaxAllowedQuantity { get; set; }

    //    [JsonProperty("activeStatus")]
    //    public string ActiveStatus { get; set; }

    //    [JsonProperty("divisor")]
    //    public int Divisor { get; set; }

    //    [JsonProperty("isPromoter")]
    //    public string IsPromoter { get; set; }

    //    [JsonProperty("cumDate")]
    //    public object CumDate { get; set; }

    //    [JsonProperty("oddLotQty")]
    //    public int OddLotQty { get; set; }

    //    [JsonProperty("cdsStockRefId")]
    //    public int CdsStockRefId { get; set; }

    //    [JsonProperty("securityName")]
    //    public string SecurityName { get; set; }

    //    [JsonProperty("tradingStartDate")]
    //    public string TradingStartDate { get; set; }

    //    [JsonProperty("networthBasePrice")]
    //    public double NetworthBasePrice { get; set; }

    //    [JsonProperty("preOpenDprHigh")]
    //    public double PreOpenDprHigh { get; set; }

    //    [JsonProperty("preOpenDprLow")]
    //    public double PreOpenDprLow { get; set; }

    //    [JsonProperty("securityTradeCycle")]
    //    public int SecurityTradeCycle { get; set; }
    //}
}
