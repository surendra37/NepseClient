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
        private readonly INepseClient _client;
        private IEnumerable<ISecurityItem> _items;
        public IEnumerable<ISecurityItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public LiveMarketPageViewModel(SocketHelper socket, INepseClient client)
        {
            _socket = socket;
            _client = client;
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
                Items = items.Data.OrderByDescending(x => x.LastTradedDateTime).ToArray();
                if (!_client.IsLive())
                {
                    // to stop fetching multiple data during not live
                    _socket.Send(_headerRequest, false);
                }
            }
            catch (Exception ex)
            {
                Log.Verbose(ex, "Failed to receive");
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

            var alreadyLoaded = Items?.Any() ?? false;

            if (_client.IsLive() || !alreadyLoaded)
                _socket.Send(_headerRequest, IsActive);
        }
        #endregion
    }
}
