using NepseApp.Extensions;
using NepseApp.Models;

using NepseClient.Commons.Contracts;

using System;
using System.Collections.Generic;

using TradeManagementSystemClient;

namespace NepseApp.ViewModels
{
    public class LiveMarketPageViewModel : ActiveAwareBindableBase
    {
        private const string _headerRequest = "top25securities";
        private readonly TmsClient _client;

        private IEnumerable<ISecurityItem> _items;
        public IEnumerable<ISecurityItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public LiveMarketPageViewModel(TmsClient client, IApplicationCommand appCommand) :
            base(appCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting live market data...");
                Items = _client.GetLiveMarket();
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
