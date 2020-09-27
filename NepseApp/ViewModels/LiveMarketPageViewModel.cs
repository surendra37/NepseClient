using NepseApp.Models;
using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeManagementSystem.Nepse;
using TradeManagementSystemClient.Models;
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
            socket.DeserializedMessageReceived += Socket_MessageReceived;
        }

        private void Socket_MessageReceived(object sender, SocketResponse[] e)
        {
            var topSecurities = e.FirstOrDefault(x => x.Header.Transaction.Equals(TmsTransactions.Securities));
            if (topSecurities == default) return;

            try
            {
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
            try
            {
                EnqueMessage("Getting live market data");
                Items = _client.GetLiveMarket().ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get live market");
            }
        }

        private void Live()
        {
            EnqueMessage("Start seeding live market data");
            IsBusy = true;
            _socket.Send(_headerRequest, IsActive);
            //_socket.SendOpCode();
        }

        private void FirstTime()
        {
            try
            {
                EnqueMessage("Getting live market data");
                Items = _client.GetLiveMarket().ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get live market");
            }
        }
    }
}
