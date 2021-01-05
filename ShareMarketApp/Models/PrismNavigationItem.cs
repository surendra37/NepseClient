
using MaterialDesignExtensions.Model;

namespace ShareMarketApp.Models
{
    public class PrismNavigationItem : FirstLevelNavigationItem, IPrismNavigationItem
    {
        public string ViewName { get; set; }
    }
}
