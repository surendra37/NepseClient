using NepseClient.Libraries.TradeManagementSystem;
using NepseClient.Libraries.TradeManagementSystem.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using System.Timers;

namespace NepseClient.Modules.TradeManagementSystem.ViewModels
{
    public class LiveMarketPageViewModel : ActiveAwareBindableBase
    {
        private readonly Timer _timer;
        private readonly TmsClient _client;

        private WsSecurityResponse[] _items;
        public WsSecurityResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public LiveMarketPageViewModel(IApplicationCommand appCommand, TmsClient client)
            : base(appCommand)
        {
            _client = client;
            _timer = new Timer
            {
                AutoReset = true,
                Interval = 10_000, //5 seconds,
            };
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RefreshCommand.Execute();
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                AppCommand.ShowMessage("Fetching data...");
                var items = await _client.GetLiveMarketAsync();
                AppCommand.ShowMessage(string.Empty);
                Items = items.Payload.Data;
                IsBusy = false;
            }
            catch (System.Exception ex)
            {
                AppCommand.ShowMessage(string.Empty);
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to refresh");
            }
        }

        protected override void OnIsActiveChanged()
        {
            base.OnIsActiveChanged();
            _timer.Enabled = IsActive;
        }
    }
}
