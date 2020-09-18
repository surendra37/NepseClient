using NepseClient.Commons;
using Newtonsoft.Json;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeManagementSystem.Nepse;
using TradeManagementSystemClient.Models.Responses;
using WebSocket4Net;

namespace NepseApp.ViewModels
{
    public class LiveMarketPageViewModel : BindableBase, IActiveAware
    {
        private const string _headerRequest = "top25securities";
        private readonly SocketHelper _socket;
        private IEnumerable<ISecurityItem> _items;
        public IEnumerable<ISecurityItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RefreshCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public LiveMarketPageViewModel(SocketHelper socket)
        {
            _socket = socket;

            socket.MessageReceived += Socket_MessageReceived;
        }

        private void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            try
            {
                var response = JsonConvert.DeserializeObject<SocketResponse[]>(e.Message);
                var topSecurities = response.FirstOrDefault(x => x.Header.Transaction.Equals(_headerRequest));
                if (topSecurities == default) return;

                var items = topSecurities.Payload.ToObject<PayloadResponse<SecurityItem>>();
                Items = items.Data.OrderByDescending(x => x.LastTradedDateTime);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Log.Verbose(ex, "Failed to receive");
            }
        }

        private DelegateCommand _refreshCommand;

        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand, () => !IsBusy));

        void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                _socket.Send(_headerRequest, true);
                //Items = _client.GetLiveMarket();
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Error(ex, "Failed to get Live market data");
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
            // UpdateCommand.IsActive = IsActive; //set the command as active
            IsActiveChanged?.Invoke(this, new EventArgs()); //invoke the event for all listeners

            _socket.Send(_headerRequest, IsActive);
        } 
        #endregion
    }
}
