using NepseClient.Modules.MeroShare.ViewModels;
using NepseClient.Modules.MeroShare.Views;

using Prism.Ioc;
using Prism.Modularity;

namespace NepseClient.Modules.MeroShare
{
    public class MeroShareModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AsbaPage, AsbaPageViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
        }
    }
}