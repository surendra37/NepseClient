using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Stocks.Models;

namespace NepseClient.Modules.Stocks.Extensions
{
    public static class SideNavigationItemExtensions
    {
        public static WatchlistSideNavigationItem AdaptToWatchlistItem(this TodayPriceContent content)
        {
            return new WatchlistSideNavigationItem
            {
                ChangeType = ChangeType.Point,
                LastTradedPrice = content.LastUpdatedPrice,
                MarketCap = content.ActualMarketCapitalization,
                PercentChange = (content.LastUpdatedPrice - content.PreviousDayClosePrice) / content.PreviousDayClosePrice,
                PointChange = content.LastUpdatedPrice - content.PreviousDayClosePrice,
                SubTitle = content.SecurityName,
                Title = content.Symbol,
            };
        }
    }
}
