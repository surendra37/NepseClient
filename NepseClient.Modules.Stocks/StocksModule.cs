using NepseClient.Commons.Constants;
using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Modules.Stocks.ViewModels;
using NepseClient.Modules.Stocks.Views;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace NepseClient.Modules.Stocks
{
    public class StocksModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ServiceClient>();

            containerRegistry.RegisterForNavigation<SideNavPage, SideNavPageViewModel>("StocksNavPage");
            containerRegistry.RegisterForNavigation<NepseNoticePage, NepseNoticePageViewModel>();
            containerRegistry.RegisterForNavigation<FloorsheetPage, FloorsheetPageViewModel>();
            containerRegistry.RegisterForNavigation<StockContentPage, StockContentPageViewModel>();
        }
    }
}