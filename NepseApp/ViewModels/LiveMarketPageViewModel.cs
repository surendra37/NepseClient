using NepseApp.Models;
using NepseClient.Commons;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeManagementSystem.Nepse;
using TradeManagementSystemClient.Models.Responses;
using WebSocket4Net;

namespace NepseApp.ViewModels
{
    public class LiveMarketPageViewModel : ActiveAwareBindableBase
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

        public LiveMarketPageViewModel(SocketHelper socket, INepseClient client, IApplicationCommand appCommand) :
            base(appCommand)
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
                    EnqueMessage("Stop seeding live market data");
                    _socket.Send(_headerRequest, false);
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Log.Verbose(ex, "Failed to receive");
            }
        }

        public override void ExecuteRefreshCommand()
        {
            var alreadyLoaded = Items?.Any() ?? false;

            if (_client.IsLive() || !alreadyLoaded)
            {
                EnqueMessage("Start seeding live market data");
                IsBusy = true;
                _socket.Send(_headerRequest, IsActive);
            }

        }
    }
}
