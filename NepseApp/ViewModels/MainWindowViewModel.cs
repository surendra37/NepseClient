using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using NepseApp.Views;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Linq;

namespace NepseApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private string _title = "Nepse App";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public IEnumerable<INavigationItem> NavigationItems => GetNavigationItem().ToArray();

        private object UpdateNavigationSelection(string source)
        {
            _regionManager.RequestNavigate("ContentRegion", source);
            return null;
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        private IEnumerable<INavigationItem> GetNavigationItem()
        {
            yield return new SubheaderNavigationItem() { Subheader = "General" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Dashboard",
                Icon = PackIconKind.ViewDashboard,
                IsSelectable = false,
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Portfolio",
                Icon = PackIconKind.Person,
                IsSelected = true,
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(PortfolioPage))
            };
            yield return new DividerNavigationItem();

            yield return new SubheaderNavigationItem() { Subheader = "Market" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Live",
                Icon = PackIconKind.RecordCircle,
                IsSelectable = false,
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Settings",
                Icon = PackIconKind.Settings,
                IsSelectable = false,
            };
        }
    }
}
