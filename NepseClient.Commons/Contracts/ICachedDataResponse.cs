using NepseClient.Commons.Contracts;

namespace NepseClient.Commons.Contracts
{
    public interface ICachedDataResponse
    {
        ICurrentMarketSession CurrentMarketSession { get; set; }
        object[] DealerSecurity { get; set; }
        IInstrumentType[] InstrumentType { get; set; }
        IMarketType[] MarketType { get; set; }
        IOperatingParameter[] OperatingParameter { get; set; }
        IOrderType[] OrderType { get; set; }
        object[] PostCloseSecurities { get; set; }
        IProductType[] ProductType { get; set; }
        ISecurityResponse[] Security { get; set; }
        IValidity[] Validity { get; set; }
    }
}