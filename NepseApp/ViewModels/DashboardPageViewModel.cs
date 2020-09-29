﻿using NepseApp.Models;
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
                Indices = await Task.Run(() => _client.GetIndices());

                TopGainers = _client.GetTopGainers();
                TopLosers = _client.GetTopLosers();

                TopTurnovers = _client.GetTopTurnovers();
                TopVolume = _client.GetTopVolumes();
                TopTransactions = _client.GetTopTransactions();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load dashboard");
            }
        }

        #region IActiveAware
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        public event EventHandler IsActiveChanged;

        private void OnIsActiveChanged()
        {
            RefreshCommand.IsActive = IsActive; //set the command as active            
            IsActiveChanged?.Invoke(this, new EventArgs()); //invoke the event for all listeners
        }
        #endregion
    }
}
