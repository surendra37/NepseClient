using NepseApp.Models;
using NepseClient.Commons;
using System;
using System.Collections.Generic;

namespace NepseApp.ViewModels
{
    public class PortfolioPageViewModel : ActiveAwareBindableBase
    {
        private readonly INepseClient _client;

        private IEnumerable<IScripResponse> _items;
        public IEnumerable<IScripResponse> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public PortfolioPageViewModel(INepseClient client, IApplicationCommand applicationCommand) :
            base(applicationCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                EnqueMessage("Refreshing portfolio");
                IsBusy = true;
                Items = _client.GetMyPortfolio();
                IsBusy = false;                
                EnqueMessage("Portfolio updated");
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to refresh portfolio");
            }
        }
    }
}
