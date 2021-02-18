using MaterialDesignExtensions.Model;

using NepseClient.Modules.Commons.Interfaces;

namespace NepseClient.Modules.Commons.Models
{
    public class PrismSideNavItem : FirstLevelNavigationItem, IViewNavigationItem
    {
        public string Target { get; }
        public PrismSideNavItem(string title, object icon, string target, bool isSelected = false)
        {
            Label = title;
            Icon = icon;
            Target = target;
            IsSelected = isSelected;
        }
    }
}
