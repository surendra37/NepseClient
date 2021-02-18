using NepseClient.Libraries.TradeManagementSystem;
using NepseClient.Libraries.TradeManagementSystem.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using System;

namespace NepseClient.Modules.TradeManagementSystem.ViewModels
{
    public class PortfolioPageViewModel : ActiveAwareBindableBase
    {
        private readonly TmsClient _client;

        private PortfolioResponse[] _items;
        public PortfolioResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }
        public PortfolioPageViewModel(TmsClient client, IApplicationCommand appCommand)
            : base(appCommand)
        {
            _client = client;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                Items = await _client.GetPortfolioAsync();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to refresh");
            }
        }
    }
}
