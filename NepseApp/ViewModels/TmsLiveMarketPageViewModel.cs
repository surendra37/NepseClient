using NepseApp.Models;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.Tms;

namespace NepseApp.ViewModels
{
    public class TmsLiveMarketPageViewModel : ActiveAwareBindableBase
    {
        private readonly TmsClient _client;

        private WsSecurityResponse[] _items;
        public WsSecurityResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public TmsLiveMarketPageViewModel(IApplicationCommand appCommand, TmsClient client)
            : base(appCommand)
        {
            _client = client;
        }
        public override void ExecuteRefreshCommand()
        {
            try
            {
                var isin = _client.GetGraphData("NPE019A00007");
                Items = _client.GetSecurities()?.Payload.Data;
            }
            catch (System.Exception ex)
            {
                LogErrorAndEnqueMessage(ex, ex.Message);
            }
        }
    }
}
