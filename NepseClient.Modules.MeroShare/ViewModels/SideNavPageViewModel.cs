using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseClient.Commons.Constants;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Views;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System.Collections.Generic;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class SideNavPageViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public IEnumerable<INavigationItem> NavigationItems
        {
            get
            {
                return new INavigationItem[]
                {
                    //new SubheaderNavigationItem() { Subheader = "Documents" },
                    new PrismSideNavItem("My Portfolio", PackIconKind.ClipboardPerson, "MeroSharePortfolioPage", true),
                    new SubheaderNavigationItem(){ Subheader = "My ASBA" },
                    new PrismSideNavItem("Application Report", PackIconKind.Web, nameof(MyAsbaApplicationReportPage)),
                    new PrismSideNavItem("Old Application Report", PackIconKind.Web, nameof(MyAsbaOldApplicationReportPage)),
                };
            }
        }

        public SideNavPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private DelegateCommand<INavigationItem> _onNavigated;
        public DelegateCommand<INavigationItem> OnNavigated =>
            _onNavigated ?? (_onNavigated = new DelegateCommand<INavigationItem>(ExecuteOnNavigated));

        void ExecuteOnNavigated(INavigationItem arg)
        {
            if (arg is null) return;
            if (arg is IViewNavigationItem item)
            {
                _regionManager.RequestNavigate(RegionNames.ContentRegion, item.Target);
            }
        }
    }
}
