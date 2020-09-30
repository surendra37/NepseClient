using NepseApp.Models;
using NepseClient.Commons.Contracts;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NepseApp.ViewModels
{
    public class DashboardPageViewModel : ActiveAwareBindableBase
    {
        private readonly INepseClient _client;

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

        public DashboardPageViewModel(IApplicationCommand applicationCommand, INepseClient client) :
            base(applicationCommand)
        {
            _client = client;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                Indices = await _client.GetIndicesAsync();

                TopGainers = await _client.GetTopGainersAsync();
                TopLosers = await _client.GetTopLosersAsync();

                TopTurnovers = await _client.GetTopTurnoversAsync();
                TopVolume = await _client.GetTopVolumesAsync();
                TopTransactions = await _client.GetTopTransactionsAsync();
                IsBusy = false;
            }
            catch(AggregateException ex)
            {
                IsBusy = false;
                _client.HandleAuthException(ex, RefreshCommand);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load dashboard");
            }
        }
    }
}
