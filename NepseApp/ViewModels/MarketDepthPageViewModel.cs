using NepseApp.Extensions;
using NepseApp.Models;
using NepseClient.Commons.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace NepseApp.ViewModels
{
    public class MarketDepthPageViewModel : ActiveAwareBindableBase
    {
        private readonly INepseClient _client;

        private ICachedDataResponse _cached;
        public ICachedDataResponse Cached
        {
            get { return _cached; }
            set { SetProperty(ref _cached, value); }
        }

        public IEnumerable<ISecurityResponse> Items => Cached?.Security;

        private ISecurityResponse _selectedItem;
        public ISecurityResponse SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public IDictionary<string, object> StockQuoteDict => ToDict(StockQuote);

        private IStockQuoteResponse _stockQuote;
        public IStockQuoteResponse StockQuote
        {
            get { return _stockQuote; }
            set { SetProperty(ref _stockQuote, value); }
        }

        public MarketDepthPageViewModel(IApplicationCommand appCommand, INepseClient client) : base(appCommand)
        {
            _client = client;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting market depth...");
                if (Cached is null)
                {
                    Cached = await _client.GetCachedData();
                    SelectedItem = Items.FirstOrDefault();
                    RaisePropertyChanged(nameof(Items));
                }
                if (SelectedItem != null)
                {
                    var stockQuote = await _client.GetStockQuoteAsync(SelectedItem?.Id.ToString());
                    StockQuote = stockQuote.FirstOrDefault();
                    RaisePropertyChanged(nameof(StockQuoteDict));
                }
                AppCommand.HideMessage();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                AppCommand.HideMessage();
                _client.HandleAuthException(ex, RefreshCommand);
            }
        }

        private IDictionary<string, object> ToDict(IStockQuoteResponse stock)
        {
            if (stock is null) return null;
            return new Dictionary<string, object>
            {
                { "Symbol", stock.Security.Symbol },
                { "Point", stock.Change },
                { "% Change", stock.ChangePercentage },
                { "LTP", stock.Ltp },
                { "Avg. Price", stock.AverageTradedPrice },
                { "Open", stock.OpenPrice },
                { "D High", stock.DayHigh },
                { "D Low", stock.DayLow },
                { "Close", stock.ClosePrice },
                { "LTQ", stock.LastTradedQty },
                { "Volume", stock.Volume },
                { "52W High", stock.Security.FiftyTwoWeekhigh },
                { "52W Low", stock.Security.FiftyTwoWeekLow },
                { "LTT", stock.LastTradedTime.ToString() },
            };
        }
    }
}
