using System.Collections.Generic;

using MaterialDesignExtensions.Model;

using Prism.Mvvm;
using Prism.Regions;

using ShareMarketApp.Models;
using ShareMarketApp.Views;

namespace ShareMarketApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private string _title = "Stocks";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public IList<INavigationItem> NavigationItems => CreateNavigationItem();

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private IList<INavigationItem> CreateNavigationItem()
        {
            var output = new List<INavigationItem>
            {
                new SubheaderNavigationItem { Subheader="MeroShare" },
                new PrismNavigationItem
                {
                    Label="My Portfolio",
                    NavigationItemSelectedCallback = Navigate,
                    ViewName = nameof(MerosharePortfolioPage)
                },
                new SubheaderNavigationItem { Subheader="TMS" },
                new PrismNavigationItem { Label="My Portfolio", NavigationItemSelectedCallback = Navigate },
            };
            return output;
        }

        private object Navigate(INavigationItem item)
        {
            if (item is IPrismNavigationItem navItem)
            {
                if (!string.IsNullOrEmpty(navItem.ViewName))
                {
                    _regionManager.RequestNavigate("ContentRegion", navItem.ViewName);
                }
            }

            return null;
        }
    }
}
