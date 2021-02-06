using NepseClient.Commons.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Stocks.Utils;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

using System;
using System.Windows.Input;

namespace NepseClient.Modules.Stocks.Adapters
{
    public class StockSideNavAdapter : BindableBase, IStockSideNavItem
    {
        private readonly WatchableTodayPrice _stock;

        public string Title => _stock.Symbol;
        public string SubTitle => _stock.SecurityName;

        public double LastTradedPrice => _stock.LastUpdatedPrice;
        public double PointChange => _stock.LastUpdatedPrice - _stock.PreviousDayClosePrice;
        public double PercentChange => PointChange / _stock.PreviousDayClosePrice;
        public double MarketCap => _stock.MarketCapitalization ?? double.NaN;
        public bool IsWatching => _stock.IsWatching;

        public string DisplayChangeText { get; init; }

        public StockSideNavAdapter(WatchableTodayPrice stock, ChangeType type, ICommand watchUpdateCommand)
        {
            _stock = stock;
            UpdateWatchingCommand = watchUpdateCommand;
            DisplayChangeText = type switch
            {
                ChangeType.PointChange => NumberUtils.GetPointChangedText(PointChange),
                ChangeType.PercentChange => NumberUtils.GetPercentChangedText(PercentChange),
                ChangeType.MarketCap => NumberUtils.GetMarketCapText(MarketCap),
                _ => "N/A",
            };
        }

        public void UpdateUI()
        {
            RaisePropertyChanged(nameof(DisplayChangeText));
        }

        public ICommand UpdateWatchingCommand { get; init; }
    }
}