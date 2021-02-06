using NepseClient.Commons.Interfaces;

namespace NepseClient.Modules.Stocks.Adapters
{

    public class NewsSideNavAdapter : INewsNavItem
    {
        public string Title { get; init; }
        public string SubTitle { get; init; }
    }
}