using System;

using NepseApp.Extensions;
using NepseApp.Models;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaApplicationReportPageViewModel : ActiveAwareBindableBase
    {
        private readonly MeroshareClient _client;

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareAsbaApplicationReportPageViewModel(IApplicationCommand appCommand, 
            MeroshareClient client) : base(appCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting application report");
                Items = _client.GetApplicationReport().Object;
                IsBusy = false;
                EnqueMessage("Application report updated successfully");
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Debug(ex, "Failed to get application report");
                EnqueMessage("Failed to get application report");
            }
            AppCommand.HideMessage();
        }
    }
}
