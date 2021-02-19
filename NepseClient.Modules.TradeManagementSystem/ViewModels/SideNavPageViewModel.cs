using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseClient.Commons.Constants;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.TradeManagementSystem.Views;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System.Collections.Generic;

namespace NepseClient.Modules.TradeManagementSystem.ViewModels
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
                    new PrismSideNavItem("My Portfolio", PackIconKind.Marketplace, nameof(PortfolioPage), true),
                    new SubheaderNavigationItem { Subheader = "Market Data" },
                    new PrismSideNavItem("Live Market", PackIconKind.XboxLive, nameof(LiveMarketPage)),
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
