using NepseApp.Models;
using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;

namespace NepseApp.ViewModels
{
    public class DashboardPageViewModel : ActiveAwareBindableBase
    {
        private readonly INepseClient _client;
        public DashboardPageViewModel(IApplicationCommand applicationCommand, INepseClient client) :
            base(applicationCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                //var res = _client.GetMarketWatch();
                var gainers = _client.GetTopGainers();
                var losers = _client.GetTopLosers();
                var trans = _client.GetTopTransactions();
                var turnovers = _client.GetTopTurnovers();
                var volumes = _client.GetTopVolumes();
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
