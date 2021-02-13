using System;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class SecurityDailyTradeDto
    {
        public string SecurityId { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public int TotalTradeQuantity { get; set; }
        public int TotalTrades { get; set; }
        public double LastTradedPrice { get; set; }
        public double PreviousClose { get; set; }
        public string BusinessDate { get; set; }
        public double? ClosePrice { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public double PointChange => LastTradedPrice - PreviousClose;
        public double AverageVolume => (double)TotalTradeQuantity / TotalTrades;
    }

    public class InstrumentType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ActiveStatus { get; set; }
    }

    public class ShareGroupId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CapitalRangeMin { get; set; }
        public object ModifiedBy { get; set; }
        public object ModifiedDate { get; set; }
        public string ActiveStatus { get; set; }
        public string IsDefault { get; set; }
    }

    public class SectorMaster
    {
        public int Id { get; set; }
        public string SectorDescription { get; set; }
        public string ActiveStatus { get; set; }
        public string RegulatoryBody { get; set; }
    }

    public class CompanyId
    {
        public int Id { get; set; }
        public string CompanyShortName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyContactPerson { get; set; }
        public SectorMaster SectorMaster { get; set; }
        public object CompanyRegistrationNumber { get; set; }
        public string ActiveStatus { get; set; }
    }

    public class Security
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Isin { get; set; }
        public string PermittedToTrade { get; set; }
        public string ListingDate { get; set; }
        public object CreditRating { get; set; }
        public double TickSize { get; set; }
        public InstrumentType InstrumentType { get; set; }
        public string CapitalGainBaseDate { get; set; }
        public double FaceValue { get; set; }
        public object HighRangeDPR { get; set; }
        public object IssuerName { get; set; }
        public int MeInstanceNumber { get; set; }
        public object ParentId { get; set; }
        public int RecordType { get; set; }
        public object SchemeDescription { get; set; }
        public object SchemeName { get; set; }
        public object Secured { get; set; }
        public object Series { get; set; }
        public ShareGroupId ShareGroupId { get; set; }
        public string ActiveStatus { get; set; }
        public int Divisor { get; set; }
        public int CdsStockRefId { get; set; }
        public string SecurityName { get; set; }
        public DateTime TradingStartDate { get; set; }
        public double NetworthBasePrice { get; set; }
        public int SecurityTradeCycle { get; set; }
        public string IsPromoter { get; set; }
        public CompanyId CompanyId { get; set; }
    }

    public class SecurityDetailResponse
    {
        public SecurityDailyTradeDto SecurityDailyTradeDto { get; set; }
        public Security Security { get; set; }
        public double StockListedShares { get; set; }
        public double PaidUpCapital { get; set; }
        public double IssuedCapital { get; set; }
        public double MarketCapitalization { get; set; }
        public int PublicShares { get; set; }
        public double PublicPercentage { get; set; }
        public double PromoterShares { get; set; }
        public double PromoterPercentage { get; set; }
        public string UpdatedDate { get; set; }
        public int SecurityId { get; set; }
    }


}
