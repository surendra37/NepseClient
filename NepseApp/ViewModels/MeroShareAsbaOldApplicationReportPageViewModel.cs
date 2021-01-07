
using System;

using NepseApp.Extensions;
using NepseApp.Models;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaOldApplicationReportPageViewModel : ActiveAwareBindableBase
    {
        private readonly MeroshareClient _client;

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareAsbaOldApplicationReportPageViewModel(IApplicationCommand appCommand, 
            MeroshareClient client) : base(appCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting old application report");
                Items = _client.GetOldApplicationReport().Object;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                EnqueMessage("Failed to get old application report");
                Log.Debug(ex, "Failed to get old application report");
            }
            AppCommand.HideMessage();
        }
    }
}
