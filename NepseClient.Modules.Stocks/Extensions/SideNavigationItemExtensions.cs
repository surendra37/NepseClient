using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Stocks.Models;

using System.Windows.Input;

namespace NepseClient.Modules.Stocks.Extensions
{
    public static class SideNavigationItemExtensions
    {
        public static WatchlistSideNavigationItem AdaptToWatchlistItem(this WatchableTodayPrice content, ICommand watchCommand)
        {
            return new WatchlistSideNavigationItem(content, watchCommand);
        }
    }
}
