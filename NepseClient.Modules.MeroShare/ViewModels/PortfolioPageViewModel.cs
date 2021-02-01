using NepseClient.Commons.Contracts;
using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Adapters;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class PortfolioPageViewModel : ActiveAwareBindableBase
    {
        private readonly MeroshareClient _client;

        private IEnumerable<IScripResponse> _items;
        public IEnumerable<IScripResponse> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        public int? TotalScrips => Items?.Count();
        public double? TotalPrevious => Items?.Sum(x => x.PreviousTotal);
        public double? TotalLTP => Items?.Sum(x => x.LTPTotal);
        public double? TotalWacc => Items?.Sum(x => x.TotalCost);
        public double? DailyGain => Items?.Sum(x => x.DailyGain);
        public double? TotalGain => Items?.Sum(x => x.TotalGain);

        public PortfolioPageViewModel(MeroshareClient client, IApplicationCommand applicationCommand) :
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
                var portfolios = await Task.Run(() => _client.GetMyPortfolios());
                var waccDict = await Task.Run(() =>_client.ReadWaccFromFile().ToDictionary(x => x.ScripName));
                var newItems = portfolios.MeroShareMyPortfolio
                    .Select(x => new MeroSharePortfolioAdapter(x, waccDict))
                    .ToArray();

                Items = newItems;
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
                LogDebugAndEnqueMessage(ex.Message);
            }
            AppCommand.HideMessage();
        }
    }
}
