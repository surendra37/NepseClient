using NepseClient.Commons.Constants;
using NepseClient.Modules.MeroShare.ViewModels;
using NepseClient.Modules.MeroShare.Views;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace NepseClient.Modules.MeroShare
{
    public class MeroShareModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaApplyForIssuePage));
            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaApplicationReportPage));
            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaOldApplicationReportPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<MyAsbaApplicationFormDialog, MyAsbaApplicationFormDialogViewModel>();
            containerRegistry.RegisterDialog<MyAsbaApplicationReportDialog, MyAsbaApplicationReportDialogViewModel>();

            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>();
            containerRegistry.RegisterForNavigation<AsbaPage, AsbaPageViewModel>();
        }
    }
}