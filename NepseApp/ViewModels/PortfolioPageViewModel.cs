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
                Items = await _client.GetMyPortfolioAsync();
                AppCommand.HideMessage();
                IsBusy = false;
                RaisePropertyChanged(nameof(TotalScrips));
                RaisePropertyChanged(nameof(TotalPrevious));
                RaisePropertyChanged(nameof(TotalLTP));
                RaisePropertyChanged(nameof(DailyGain));
                RaisePropertyChanged(nameof(TotalWacc));
                RaisePropertyChanged(nameof(TotalGain));
            }
            catch (AggregateException ex)
            {
                IsBusy = false;
                _client.HandleAuthException(ex, RefreshCommand);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to update portfolio");
            }
        }
    }
}
