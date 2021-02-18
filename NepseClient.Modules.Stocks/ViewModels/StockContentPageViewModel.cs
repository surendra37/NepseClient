using LiveCharts;
using LiveCharts.Configurations;

using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;
using Prism.Regions;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class StockContentPageViewModel : ActiveAwareBindableBase, INavigationAware
    {
        private readonly ServiceClient _client;
        private int _id;

        #region Details
        private CorporteActionResponse[] _corporteActions;
        public CorporteActionResponse[] CorporteActions
        {
            get { return _corporteActions; }
            set { SetProperty(ref _corporteActions, value); }
        }

        private SecurityDetailResponse _security;
        public SecurityDetailResponse Security
        {
            get { return _security; }
            set { SetProperty(ref _security, value); }
        }
        #endregion
        public Func<double, string> Formatter { get; }
        public CartesianMapper<GraphResponse> ClosePriceConfiguration { get; }
        public FinancialMapper<GraphResponse> OhlcConfiguration { get; }

        public GraphResponse[] Items { get; private set; }

        private ChartValues<GraphResponse> _closePriceValues;
        public ChartValues<GraphResponse> ClosePriceValues
        {
            get { return _closePriceValues; }
            set { SetProperty(ref _closePriceValues, value); }
        }

        private double _lastPrice;
        public double LastPrice
        {
            get { return _lastPrice; }
            set { SetProperty(ref _lastPrice, value); }
        }

        private bool _isOhlcVisible;
        public bool IsOhlcVisible
        {
            get { return _isOhlcVisible; }
            set { SetProperty(ref _isOhlcVisible, value); }
        }

        private string _selectedFilter = "1W";
        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set { SetProperty(ref _selectedFilter, value); }
        }

        public StockContentPageViewModel(IApplicationCommand appCommand, ServiceClient client)
                : base(appCommand)
        {
            _client = client;

            ClosePriceConfiguration =
                Mappers.Xy<GraphResponse>()
                .X(x => x.BusinessDate.Ticks / TimeSpan.FromDays(1).Ticks)
                .Y(y => y.ClosePrice);

            OhlcConfiguration = Mappers.Financial<GraphResponse>()
                .Open(x => x.OpenPrice)
                .High(x => x.HighPrice)
                .Low(x => x.LowPrice)
                .Close(x => x.ClosePrice)
                .X(x => x.BusinessDate.Ticks / TimeSpan.FromDays(1).Ticks)
                .Y(x => x.ClosePrice);

            ClosePriceValues = new ChartValues<GraphResponse>();

            Formatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var success = navigationContext.Parameters.TryGetValue("Id", out _id);
            if (!success) return;
            RefreshCommand.Execute();
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                UpdateData();
                UpdateUI();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Error(ex, "Failed to refresh content");
            }
        }

        private DelegateCommand<string> _filterGraphCommand;
        public DelegateCommand<string> FilterGraphCommand =>
            _filterGraphCommand ?? (_filterGraphCommand = new DelegateCommand<string>(ExecuteFilterGraphCommand));

        void ExecuteFilterGraphCommand(string param)
        {
            SelectedFilter = param;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var ohlcVisibleOn = new string[] { "1D", "1W", "1M" };
            IsOhlcVisible = ohlcVisibleOn.Contains(SelectedFilter);
            IEnumerable<GraphResponse> filteredData = SelectedFilter switch
            {
                "1D" => Items.TakeLast(2),
                "1W" => Items.TakeLast(7),
                "1M" => Items.TakeLast(30),
                "3M" => Items.TakeLast(90),
                "6M" => Items.TakeLast(180),
                "1Y" => Items.TakeLast(365),
                "2Y" => Items.TakeLast(365 * 2),
                "5Y" => Items.TakeLast(365 * 5),
                "10Y" => Items.TakeLast(365 * 10),
                "All" => Items,
                _ => Items,
            };
            ClosePriceValues.Clear();
            ClosePriceValues.AddRange(filteredData);
        }

        private void UpdateData()
        {
            Security = _client.GetSecurityDetail(_id);
            Items = _client.GetGraphData(_id);
            CorporteActions = _client.GetCorporateActions(_id);
            LastPrice = Items.LastOrDefault()?.ClosePrice ?? 0;
        }
    }
}
