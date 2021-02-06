using NepseClient.Commons.Constants;
using NepseClient.Commons.Interfaces;
using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Modules.Stocks.Adapters;
using NepseClient.Modules.Stocks.Views;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using Serilog;

using System;
using System.Linq;
using System.Windows.Threading;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class DashboardPageViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ServiceClient _client;
        private readonly DatabaseContext _context;

        private string _marketStatusText = "Market Open";
        public string MarketStatusText
        {
            get { return _marketStatusText; }
            set { SetProperty(ref _marketStatusText, value); }
        }

        #region Menu Items
        public bool IsPriceChangedSelected
        {
            get => ChangeType == ChangeType.PointChange;
            set
            {
                if (value)
                {
                    ChangeType = ChangeType.PointChange;
                    //RaisePropertyChanged(nameof(IsPriceChangedSelected));
                    RaisePropertyChanged(nameof(IsPercentageChangedSelected));
                    RaisePropertyChanged(nameof(IsMarketCapSelected));
                }
            }
        }

        public bool IsPercentageChangedSelected
        {
            get => ChangeType == ChangeType.PercentChange;
            set
            {
                if (value)
                {
                    ChangeType = ChangeType.PercentChange;
                    RaisePropertyChanged(nameof(IsPriceChangedSelected));
                    //RaisePropertyChanged(nameof(IsPercentageChangedSelected));
                    RaisePropertyChanged(nameof(IsMarketCapSelected));
                }
            }
        }

        public bool IsMarketCapSelected
        {
            get => ChangeType == ChangeType.MarketCap;
            set
            {
                if (value)
                {
                    ChangeType = ChangeType.MarketCap;
                    RaisePropertyChanged(nameof(IsPriceChangedSelected));
                    RaisePropertyChanged(nameof(IsPercentageChangedSelected));
                    //RaisePropertyChanged(nameof(IsMarketCapSelected));
                }
            }
        }

        private ChangeType changeType = ChangeType.PointChange;
        public ChangeType ChangeType
        {
            get { return changeType; }
            set
            {
                if (SetProperty(ref changeType, value))
                {
                    Update();
                }
            }
        }
        #endregion

        #region Side Nav
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    SearchCommand.Execute();
                }
            }
        }

        public ISideNavItem[] AllItems { get; set; }
        public ISideNavItem[] FilteredItems => string.IsNullOrWhiteSpace(SearchText) ?
            AllItems.Where(WatchlistFilter).ToArray() :
            AllItems.Where(SearchFilter)
            .Cast<IStockSideNavItem>()
            .OrderByDescending(x => x.IsWatching).ToArray();

        private ISideNavItem _selectedItem;
        public ISideNavItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    UpdateContentPage(value);
                }
            }
        }

        private void UpdateContentPage(ISideNavItem value)
        {
            if (value is INewsNavItem news)
            {
                _regionManager.RequestNavigate(RegionNames.StocksRegion, nameof(NewsAndAlertPage));
            }
            else if (value is IStockSideNavItem stocks)
            {

            }
        }

        private bool SearchFilter(ISideNavItem item)
        {
            if (item is INewsNavItem)
                return false;

            if (item is IStockSideNavItem stock)
            {
                if (SearchText.Equals(stock.Title, StringComparison.OrdinalIgnoreCase)) return true;

                if (stock.SubTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) return true;
                return false;
            }
            return true;
        }
        private bool WatchlistFilter(ISideNavItem item)
        {
            if (item is INewsNavItem)
                return true;

            if (item is IStockSideNavItem stock)
            {
                return stock.IsWatching;
            }
            return true;
        }
        #endregion

        public DashboardPageViewModel(IRegionManager regionManager, ServiceClient client, DatabaseContext context)
        {
            _regionManager = regionManager;
            _client = client;
            _context = context;

            Update();
            Timer_Tick(this, EventArgs.Empty);
            var timer = new DispatcherTimer(new TimeSpan(0, 0, 10), DispatcherPriority.Background,
                    Timer_Tick, Dispatcher.CurrentDispatcher);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // check for market status
            var status = _client.GetMarketStatus();
            if (status.IsClosed)
            {
                MarketStatusText = "Market Closed";
            }
            else
            {
                MarketStatusText = "Market Open";
                Update();
            }
        }

        private void Update()
        {
            try
            {
                AllItems = _context.TodayPrice.GetWatchables()
                      .Select(x => new StockSideNavAdapter(x, ChangeType, AddCommand))
                      .Prepend<ISideNavItem>(new NewsSideNavAdapter { Title = "Business News", SubTitle = "From Nepal Stock Exchange" })
                      .ToArray();

                // Refresh UI
                RefreshUI();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update");
            }
        }

        private void RefreshUI()
        {
            try
            {
                // Refresh UI
                RaisePropertyChanged(nameof(FilteredItems));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to refresh UI");
            }
        }

        private DelegateCommand _refreshCommand;

        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        void ExecuteRefreshCommand()
        {
            try
            {

            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to refresh dashboard");
            }
        }

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand(ExecuteSearchCommand));

        void ExecuteSearchCommand()
        {
            RefreshUI();
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SearchText = string.Empty;
            SearchCommand.Execute();
        }

        private DelegateCommand<IStockSideNavItem> _addCommand;
        public DelegateCommand<IStockSideNavItem> AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand<IStockSideNavItem>(ExecuteAddCommand));

        void ExecuteAddCommand(IStockSideNavItem parameter)
        {
            if (parameter is null) return;

            try
            {
                _context.Watchlist.Update(parameter.Title, !parameter.IsWatching);
                Update();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update watching");
            }
        }
    }
}
