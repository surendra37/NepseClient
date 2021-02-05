using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Stocks.Extensions;
using NepseClient.Modules.Stocks.Models;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class DashboardPageViewModel : BindableBase
    {
        private readonly ServiceClient _client;
        private readonly DatabaseContext _context;

        private string _marketStatusText = "Market Open";
        public string MarketStatusText
        {
            get { return _marketStatusText; }
            set { SetProperty(ref _marketStatusText, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
            }
        }

        public SideNavigationItem[] Items => string.IsNullOrWhiteSpace(SearchText) ? WatchingItems : SearchItems;

        #region Menu Items
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
        #endregion

        #region Side Nav Watching
        public IEnumerable<TodayPriceContent> AllPrices { get; set; }
        public IEnumerable<TodayPriceContent> WatchingPrices { get; set; }
        public SideNavigationItem[] WatchingItems => WatchingPrices
            .Select(x => x.AdaptToWatchlistItem())
            .Prepend<SideNavigationItem>(new BusinessNewsSideNavigationItem())
            .ToArray();

        public SideNavigationItem[] SearchItems => AllPrices
            .Where(Filter)
            .Select(x => x.AdaptToWatchlistItem())
            .ToArray();
        #endregion

        private bool Filter(TodayPriceContent content)
        {
            if (SearchText.Equals(content.Symbol, System.StringComparison.OrdinalIgnoreCase)) return true;

            if (content.SecurityName.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public DashboardPageViewModel(ServiceClient client, DatabaseContext context)
        {
            _client = client;
            _context = context;

            //Update();
            RefreshUI();
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
                var prices = _client.GetTodaysPriceAll();
                // clear database
                _context.TodayPrice.Delete();

                // insert to database
                _context.TodayPrice.AddTodayPrice(prices);

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
                AllPrices = _context.TodayPrice.Get();
                WatchingPrices = _context.TodayPrice.Get(_context.Watchlist.Get());
                RaisePropertyChanged(nameof(WatchingItems));
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
            RaisePropertyChanged(nameof(Items));
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SearchText = string.Empty;
            SearchCommand.Execute();
        }
    }
}
