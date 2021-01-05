
using System.Collections;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using NepseService.TradeManagementSystem;
using NepseService.TradeManagementSystem.Models.Requests;
using NepseService.TradeManagementSystem.Models.Responses;

using Prism.Commands;
using Prism.Mvvm;

namespace ShareMarketApp.ViewModels
{
    public class MerosharePortfolioPageViewModel : BindableBase
    {
        private readonly MeroshareClient _meroShare;
        private readonly GetMyPortfolioRequest _request;

        private IEnumerable<MeroShareMyPortfolio> _items;
        public IEnumerable<MeroShareMyPortfolio> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MerosharePortfolioPageViewModel(IConfiguration config, MeroshareClient meroShare)
        {
            _meroShare = meroShare;
            _request = new GetMyPortfolioRequest
            {
                ClientCode = config["Meroshare:ClientCode"],
                Demat = new string[] { config["Meroshare:Demat"] },
                Page = 1,
                Size = 200,
                SortAsc = true,
                SortBy = "script"
            };
            RefreshCommand.Execute();
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        void ExecuteRefreshCommand()
        {
            try
            {
                var portfolio = _meroShare.GetMyPortfolio(_request);
                Items = portfolio.MeroShareMyPortfolio;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to get portfolios");
            }
        }
    }
}
