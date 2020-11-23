using NepseApp.Extensions;
using NepseApp.Models;
using NepseClient.Commons.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NepseApp.ViewModels
{
    public class PortfolioPageViewModel : ActiveAwareBindableBase
    {
        private readonly INepseClient _client;

        private IEnumerable<IScripResponse> _items;
        public IEnumerable<IScripResponse> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        public int TotalScrips => Items.Count();
        public float TotalPrevious => Items.Sum(x => x.PreviousTotal);
        public float TotalLTP => Items.Sum(x => x.LTPTotal);
        public float TotalWacc => Items.Sum(x => x.WaccValue * x.CurrentBalance);
        public float DailyGain => TotalLTP - TotalPrevious;
        public float TotalGain => TotalLTP - TotalWacc;

        public PortfolioPageViewModel(INepseClient client, IApplicationCommand applicationCommand) :
            base(applicationCommand)
        {
            _client = client;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Loading portfolio...");
                var myPortfolio = (await _client.GetMyPortfolioAsync()).ToArray();
                if (!_client.IsLive())
                {
                    var portfolio = await _client.GetOHLCPortfolioAsync();
                    var closePriceDict = portfolio.ToDictionary(x => x.Symbol, x => x.ClosePrice);
                    foreach (var p in myPortfolio)
                    {
                        if (closePriceDict.ContainsKey(p.Scrip))
                        {
                            var pClose = closePriceDict[p.Scrip];
                            if (pClose == 0) continue; // Skip for not traded scrip
                            p.PreviousClosePrice = pClose;
                            p.PreviousTotal = pClose * p.TotalBalance;
                        }
                    }
                }
                Items = myPortfolio;
                AppCommand.HideMessage();
                IsBusy = false;
                RaisePropertyChanged(nameof(TotalScrips));
                RaisePropertyChanged(nameof(TotalPrevious));
                RaisePropertyChanged(nameof(TotalLTP));
                RaisePropertyChanged(nameof(DailyGain));
                RaisePropertyChanged(nameof(TotalWacc));
                RaisePropertyChanged(nameof(TotalGain));
            }
            catch (Exception ex)
            {
                IsBusy = false;
                AppCommand.HideMessage();
                _client.HandleAuthException(ex, RefreshCommand);
            }
        }
    }
}
