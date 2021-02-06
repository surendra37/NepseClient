using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Stocks.Utils;

using System.Windows.Input;

namespace NepseClient.Modules.Stocks.Models
{

    public class WatchlistSideNavigationItem : SideNavigationItem
    {
        private readonly WatchableTodayPrice _price;

        public double LastTradedPrice => _price.LastUpdatedPrice;
        public double PointChange => _price.LastUpdatedPrice - _price.PreviousDayClosePrice;
        public double PercentChange => PointChange / _price.PreviousDayClosePrice;
        public double MarketCap => _price.MarketCapitalization ?? double.NaN;
        public bool IsWatching => _price.IsWatching;

        private ChangeType _changeType = ChangeType.Point;
        public ChangeType ChangeType
        {
            get { return _changeType; }
            set
            {
                if (SetProperty(ref _changeType, value))
                {
                    RaisePropertyChanged(nameof(DisplayChangeText));
                }
            }
        }

        public string DisplayChangeText
        {
            get
            {
                return ChangeType switch
                {
                    ChangeType.Point => NumberUtils.GetPointChangedText(PointChange),
                    ChangeType.Percent => NumberUtils.GetPercentChangedText(PercentChange),
                    ChangeType.MarketCap => NumberUtils.GetMarketCapText(MarketCap),
                    _ => "N/A",
                };
            }
        }

        public ICommand UpdateWatchingCommand { get; }

        public WatchlistSideNavigationItem(WatchableTodayPrice price, ICommand watchCommand)
        {
           _price = price;
            UpdateWatchingCommand = watchCommand;
            Title = _price.Symbol;
            SubTitle = _price.SecurityName;
        }
    }
}