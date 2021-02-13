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
            var regionManager = containerProvider.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DashboardPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ServiceClient>();

            containerRegistry.RegisterForNavigation<DashboardPage, DashboardPageViewModel>();
            containerRegistry.RegisterForNavigation<NewsAndAlertPage, NewsAndAlertPageViewModel>();
            containerRegistry.RegisterForNavigation<StockContentPage, StockContentPageViewModel>();
        }
    }
}