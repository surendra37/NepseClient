﻿using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;
using NepseClient.Commons.Interfaces;
using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.Stocks.Adapters;
using NepseClient.Modules.Stocks.Views;

using Prism.Commands;
using Prism.Regions;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class DashboardPageViewModel : ActiveAwareBindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IConfiguration _configuration;
        private readonly ServiceClient _client;
        private readonly DatabaseContext _context;

        private string _marketStatusText = "Market Open";
        public string MarketStatusText
        {
            get { return _marketStatusText; }
            set { SetProperty(ref _marketStatusText, value); }
        }
        public bool ShowNepseNotice
        {
            get => _configuration.ShowNepseNotice;
            set
            {
                if (_configuration.ShowNepseNotice != value)
                {
                    _configuration.ShowNepseNotice = value;
                    UpdateUI();
                }
            }
        }

        public bool AutoRefreshOnLoad
        {
            get => _configuration.AutoRefreshOnLoad;
            set => _configuration.AutoRefreshOnLoad = value;
        }

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

        public NewsSideNavAdapter NepseNotice { get; } = new NewsSideNavAdapter { Title = "Notices", SubTitle = "From Nepse" };

        public object[] FilteredItems
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    var list = new List<object>();
                    if (ShowNepseNotice)
                    {
                        list.Add(NepseNotice);
                    }
                    if (Items != null)
                    {
                        list.AddRange(Items.Where(x => x.IsWatching));
                    }
                    return list.ToArray();
                }
                else
                {
                    return Items?.Where(SearchFilter)
                        .OrderByDescending(x => x.IsWatching).ToArray();
                }
            }
        }

        private object _selectedItem;
        public object SelectedItem
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

        private void UpdateContentPage(object value)
        {
            if (value is INewsNavItem news)
            {
                _regionManager.RequestNavigate(RegionNames.StocksRegion, nameof(NewsAndAlertPage));
            }
            else if (value is SecurityStatResponse stocks)
            {
                var p = new NavigationParameters
                {
                    { "Stock", stocks }
                };
                _regionManager.RequestNavigate(RegionNames.StocksRegion, nameof(StockContentPage), p);
            }
        }

        private bool SearchFilter(SecurityStatResponse item)
        {
            if (SearchText.Equals(item.Symbol, StringComparison.OrdinalIgnoreCase)) return true;

            if (item.SecurityName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private SecurityStatResponse[] _items;
        public SecurityStatResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        #endregion

        public DashboardPageViewModel(IRegionManager regionManager, IApplicationCommand appCommand, IConfiguration configuration,
            ServiceClient client, DatabaseContext context)
            : base(appCommand)
        {
            _regionManager = regionManager;
            _configuration = configuration;
            _client = client;
            _context = context;

            Timer_Tick(this, EventArgs.Empty);
            var timer = new DispatcherTimer(new TimeSpan(0, 0, 10), DispatcherPriority.Background,
                    Timer_Tick, Dispatcher.CurrentDispatcher);
            //RefreshCommand.Execute();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // check for market status
            var status = _client.GetMarketStatus();
            var isClosed = status?.IsClosed ?? true;
            if (isClosed)
            {
                MarketStatusText = "Market Closed";
            }
            else
            {
                MarketStatusText = "Market Open";
                UpdateData();
                UpdateUI();
            }
        }

        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
            _searchCommand ?? (_searchCommand = new DelegateCommand(ExecuteSearchCommand));

        void ExecuteSearchCommand()
        {
            RaisePropertyChanged(nameof(FilteredItems));
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            SearchText = string.Empty;
            SearchCommand.Execute();
        }

        private DelegateCommand<SecurityStatResponse> _addCommand;
        public DelegateCommand<SecurityStatResponse> AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand<SecurityStatResponse>(ExecuteAddCommand));

        void ExecuteAddCommand(SecurityStatResponse parameter)
        {
            if (parameter is null) return;

            try
            {
                _context.Watchlist.Update(parameter.Symbol, !parameter.IsWatching);
                UpdateUI();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update watching");
            }
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                UpdateData();
                UpdateUI();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to refresh dashboard");
            }
        }

        private void UpdateUI()
        {
            if (Items is null)
            {
                RaisePropertyChanged(nameof(FilteredItems));
                return;
            }

            var watchlist = _context.Watchlist.Get();
            foreach (var item in Items)
            {
                var watching = watchlist.Contains(item.Symbol);
                item.IsWatching = watching;
            }
            RaisePropertyChanged(nameof(FilteredItems));
        }

        private void UpdateData()
        {
            Items = _client.GetDailyTradStats();
            foreach (var item in Items)
            {
                item.UpdateWatchingCommand = AddCommand;
            }
        }
    }
}
