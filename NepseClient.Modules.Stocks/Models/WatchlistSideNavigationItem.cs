using NepseClient.Modules.Stocks.Utils;

namespace NepseClient.Modules.Stocks.Models
{

    public class WatchlistSideNavigationItem : SideNavigationItem
    {
        private double _lastTradedPrice;
        public double LastTradedPrice
        {
            get { return _lastTradedPrice; }
            set { SetProperty(ref _lastTradedPrice, value); }
        }

        private double _pointChange;
        public double PointChange
        {
            get { return _pointChange; }
            set { SetProperty(ref _pointChange, value); }
        }

        private double _percentChange;
        public double PercentChange
        {
            get { return _percentChange; }
            set { SetProperty(ref _percentChange, value); }
        }

        private double _marketCap;
        public double MarketCap
        {
            get { return _marketCap; }
            set { SetProperty(ref _marketCap, value); }
        }

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
    }
}