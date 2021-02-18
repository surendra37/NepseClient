using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseClient.Commons.Constants;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Views;

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
                return new List<INavigationItem>
                {
                    //new SubheaderNavigationItem() { Subheader = "Documents" },
                    new PrismSideNavItem("My Portfolio", PackIconKind.Marketplace, nameof(PortfolioPage), true) { NavigationItemSelectedCallback=OnSelected},
                };
            }
        }

        private object OnSelected(INavigationItem navigationItem)
        {
            if (navigationItem is PrismSideNavItem item)
            {
                _regionManager.RequestNavigate(RegionNames.ContentRegion, item.Target);
            }

            return navigationItem;
        }

        public SideNavPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
    }
}
