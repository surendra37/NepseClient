using Prism.Mvvm;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.Tms;

namespace NepseClient.Modules.TradeManagementSystem.ViewModels
{
    public class TopGainerPageViewModel : BindableBase
    {
        private TopGainerResponse[] _items;
        public TopGainerResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public TopGainerPageViewModel(TmsClient client)
        {
            Items = client.GetTopGainers();
        }
    }
}
