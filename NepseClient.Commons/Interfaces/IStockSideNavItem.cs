namespace NepseClient.Commons.Interfaces
{
    public interface IStockSideNavItem : ISideNavItem
    {
        double LastTradedPrice { get; }
        double PointChange { get; }
        double PercentChange { get; }
        double MarketCap { get; }
        bool IsWatching { get; }
        void UpdateUI();
    }

}
