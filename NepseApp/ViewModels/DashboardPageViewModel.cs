using NepseApp.Extensions;
using NepseApp.Models;
using NepseClient.Commons.Contracts;
using Serilog;
using System;
using System.Collections.Generic;

using TradeManagementSystemClient;

namespace NepseApp.ViewModels
{
    public class DashboardPageViewModel : ActiveAwareBindableBase
    {
        private readonly TmsClient _client;

        private IEnumerable<IIndexResponse> _indices;
        public IEnumerable<IIndexResponse> Indices
        {
            get { return _indices; }
            set { SetProperty(ref _indices, value); }
        }

        private IEnumerable<ITopSecuritiesResponse> _topTurnovers;
        public IEnumerable<ITopSecuritiesResponse> TopTurnovers
        {
            get { return _topTurnovers; }
            set { SetProperty(ref _topTurnovers, value); }
        }

        private IEnumerable<ITopSecuritiesResponse> _topVolume;
        public IEnumerable<ITopSecuritiesResponse> TopVolume
        {
            get { return _topVolume; }
            set { SetProperty(ref _topVolume, value); }
        }

        private IEnumerable<ITopSecuritiesResponse> _topTransactions;
        public IEnumerable<ITopSecuritiesResponse> TopTransactions
        {
            get { return _topTransactions; }
            set { SetProperty(ref _topTransactions, value); }
        }

        private IEnumerable<ITopResponse> _topGainers;
        public IEnumerable<ITopResponse> TopGainers
        {
            get { return _topGainers; }
            set { SetProperty(ref _topGainers, value); }
        }

        private IEnumerable<ITopResponse> _topLosers;
        public IEnumerable<ITopResponse> TopLosers
        {
            get { return _topLosers; }
            set { SetProperty(ref _topLosers, value); }
        }

        public DashboardPageViewModel(IApplicationCommand applicationCommand, TmsClient client) :
            base(applicationCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Loading dashboard...");
                Indices =  _client.GetIndices();

                TopGainers =  _client.GetTopGainers();
                TopLosers =  _client.GetTopLosers();

                TopTurnovers =  _client.GetTopTurnovers();
                TopVolume =  _client.GetTopVolumes();
                TopTransactions =  _client.GetTopTransactions();
                AppCommand.HideMessage();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                AppCommand.HideMessage();
            }
        }
    }
}
