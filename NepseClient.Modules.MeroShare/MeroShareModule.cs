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
            regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(LoginPage));

            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaApplyForIssuePage));
            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaApplicationReportPage));
            regionManager.RegisterViewWithRegion(RegionNames.MyAsbaTabRegion, typeof(MyAsbaOldApplicationReportPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SideNavPage, SideNavPageViewModel>("MeroShareNavPage");

            containerRegistry.RegisterDialog<MyAsbaApplicationFormDialog, MyAsbaApplicationFormDialogViewModel>();
            containerRegistry.RegisterDialog<MyAsbaApplicationReportDialog, MyAsbaApplicationReportDialogViewModel>();
            containerRegistry.RegisterDialog<UpdateClientDialog, UpdateClientDialogViewModel>();

            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<PortfolioPage, PortfolioPageViewModel>("MeroSharePortfolioPage");
            containerRegistry.RegisterForNavigation<AsbaPage, AsbaPageViewModel>();

            containerRegistry.RegisterForNavigation<MyAsbaApplicationReportPage, MyAsbaApplicationReportPageViewModel>();
            containerRegistry.RegisterForNavigation<MyAsbaApplyForIssuePage, MyAsbaApplyForIssuePageViewModel>();
            containerRegistry.RegisterForNavigation<MyAsbaOldApplicationReportPage, MyAsbaOldApplicationReportPageViewModel>();
        }
    }
}