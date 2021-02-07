using NepseClient.Commons.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Regions;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class StockContentPageViewModel : ActiveAwareBindableBase, INavigationAware
    {
        private readonly DatabaseContext _context;
        private IStockSideNavItem _navItem;

        private WatchableTodayPrice _price;
        public WatchableTodayPrice Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }
        public StockContentPageViewModel(IApplicationCommand appCommand, DatabaseContext context)
            : base(appCommand)
        {
            this._context = context;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var success = navigationContext.Parameters.TryGetValue("Stock", out _navItem);

            Price = _context.TodayPrice.Find(_navItem.Title);
        }
    }
}
