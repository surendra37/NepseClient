using NepseClient.Modules.TradeManagementSystem.ViewModels;
using NepseClient.Modules.TradeManagementSystem.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace NepseClient.Modules.TradeManagementSystem
{
    public class TradeManagementSystemModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SideNavPage, SideNavPageViewModel>("TmsNavPage");
            containerRegistry.RegisterForNavigation<TopSecuritiesPage, TopSecuritiesPageViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
            containerRegistry.RegisterForNavigation<LiveMarketPage, LiveMarketPageViewModel>();
        }
    }
}