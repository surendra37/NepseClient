using NepseClient.Commons.Interfaces;

namespace NepseClient.Modules.Stocks.Adapters
{
    public class SideNavItem : INewsNavItem
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string ViewName { get; set; }
    }
}