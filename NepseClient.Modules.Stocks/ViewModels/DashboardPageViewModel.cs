using NepseClient.Modules.Stocks.Models;
using NepseClient.Modules.Stocks.Utils;

using Prism.Commands;
using Prism.Mvvm;

using System.Linq;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class DashboardPageViewModel : BindableBase
    {
        private string _marketStatusText = "Market Open";
        public string MarketStatusText
        {
            get { return _marketStatusText; }
            set { SetProperty(ref _marketStatusText, value); }
        }

        private SideNavigationItem[] _items;
        public SideNavigationItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private bool _isPriceChangedSelected = true;
        public bool IsPriceChangedSelected
        {
            get { return _isPriceChangedSelected; }
            set
            {
                if (SetProperty(ref _isPriceChangedSelected, value) && value)
                {
                    IsPercentageChangedSelected = false;
                    IsMarketCapSelected = false;
                    UpdateChangeText();
                }
            }
        }

        private bool _isPercentageChangedSelected;
        public bool IsPercentageChangedSelected
        {
            get { return _isPercentageChangedSelected; }
            set
            {
                if (SetProperty(ref _isPercentageChangedSelected, value) && value)
                {
                    IsPriceChangedSelected = false;
                    IsMarketCapSelected = false;
                    UpdateChangeText();
                }
            }
        }

        private bool _isMarketCapSelected;
        public bool IsMarketCapSelected
        {
            get { return _isMarketCapSelected; }
            set
            {
                if (SetProperty(ref _isMarketCapSelected, value) && value)
                {
                    IsPriceChangedSelected = false;
                    IsPercentageChangedSelected = false;
                    UpdateChangeText();
                }
            }
        }

        private void UpdateChangeText()
        {
            var items = Items.Where(x => x is WatchlistSideNavigationItem)
                 .Cast<WatchlistSideNavigationItem>();
            foreach (var item in items)
            {
                if (IsPriceChangedSelected)
                {
                    item.ChangeType = ChangeType.Point;
                }
                else if (IsPercentageChangedSelected)
                {
                    item.ChangeType = ChangeType.Percent;
                }
                else if (IsMarketCapSelected)
                {
                    item.ChangeType = ChangeType.MarketCap;
                }
            }
        }

        public DashboardPageViewModel()
        {
            Items = new SideNavigationItem[]
            {
                new BusinessNewsSideNavigationItem(),
                new WatchlistSideNavigationItem
                {
                    Title= "DIS",
                    SubTitle = "The Walt Disney Company",
                    LastTradedPrice = 176.96,
                    PointChange = 5.99,
                    PercentChange =0.035,
                    MarketCap = 310_000_000_000,
                }
            };


        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        void ExecuteRefreshCommand()
        {

        }
    }
}
