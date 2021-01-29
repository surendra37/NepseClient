using NepseClient.Commons.Constants;
using NepseClient.Modules.TradeManagementSystem.ViewModels;
using NepseClient.Modules.TradeManagementSystem.Views;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace NepseClient.Modules.TradeManagementSystem
{
    public class TradeManagementSystemModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.TopSecuritiesRegion, typeof(TopGainerPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TopSecuritiesPage, TopSecuritiesPageViewModel>();
        }
    }
}