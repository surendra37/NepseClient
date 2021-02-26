using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Requests;
using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class PortfolioPageViewModel : PaginationBase
    {
        private readonly int _size = 200;
        private readonly MeroshareClient _client;
        private readonly WaccContext _database;
        private IEnumerable<MeroShareMyPortfolio> _items;
        public IEnumerable<MeroShareMyPortfolio> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public int? TotalItems => Items?.Count();
        public double? TotalLtpValue => Items?.Sum(x => x.ValueOfLastTransPrice);
        public double? TotalPreviousValue => Items?.Sum(x => x.ValueOfPrevClosingPrice);
        public double? DailyProfit => TotalLtpValue - TotalPreviousValue;
        public double? TotalProfit => Items?.Sum(x => x.TotalProfit);

        public PortfolioPageViewModel(MeroshareClient client, IApplicationCommand applicationCommand, DatabaseContext database)
            : base(applicationCommand)
        {
            _client = client;
            _database = database.Wacc;
            //RefreshCommand.Execute();
        }

        protected override async void Navigate(int page)
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Loading portfolio...");
                var portfolios = await _client.GetMyPortfoliosAsync(page, _size);
                if (portfolios is not null && portfolios.TotalItems > 0)
                {
                    foreach (var item in portfolios.MeroShareMyPortfolio)
                    {
                        item.AverageBuyRate = await _database.GetBuyRate(item.Script);
                    }
                    Items = portfolios.MeroShareMyPortfolio;
                    CurrentPage = page;
                    double total = portfolios.TotalItems;
                    double count = Math.Ceiling(total / _size);
                    TotalPage = (int)count;
                }

                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogDebugAndEnqueMessage(ex.Message);
            }
            AppCommand.HideMessage();
            base.Navigate(page);

            RaisePropertyChanged(nameof(TotalItems));
            RaisePropertyChanged(nameof(TotalLtpValue));
            RaisePropertyChanged(nameof(TotalPreviousValue));
            RaisePropertyChanged(nameof(DailyProfit));
            RaisePropertyChanged(nameof(TotalProfit));
        }

        private DelegateCommand _loadWaccCommand;
        public DelegateCommand LoadWaccCommand =>
            _loadWaccCommand ?? (_loadWaccCommand = new DelegateCommand(ExecuteLoadWaccCommand));

        async void ExecuteLoadWaccCommand()
        {
            try
            {
                if (Items is null) return;

                AppCommand.ShowMessage("Loading wacc...");
                foreach (var item in Items)
                {
                    var request = new MeroshareViewMyPurchaseRequest
                    {
                        Demat = _client.Me.Demat,
                        Scrip = item.Script,
                    };
                    var purchase = await _client.ViewMyPurchaseAsync(request);
                    if (purchase is null)
                    {
                        purchase = new MeroshareViewMyPurchaseResponse
                        {
                            ScripName = item.Script,
                        };
                    }
                    var searches = await _client.SearchMyPurchase(request);
                    if (searches is null || searches.Length == 0)
                    {

                    }
                    else
                    {
                        foreach (var search in searches)
                        {
                            purchase.TotalCost += search.UserPrice * search.TransactionQuantity;
                            purchase.TotalQuantity += search.TransactionQuantity;
                            purchase.Isin = search.Isin;
                            purchase.ScripName = search.Scrip;
                        }
                        purchase.AverageBuyRate = purchase.TotalCost / purchase.TotalQuantity;
                    }
                    item.AverageBuyRate = purchase.AverageBuyRate;
                    await _database.Insert(purchase);
                }
                RaisePropertyChanged(nameof(Items));
                RaisePropertyChanged(nameof(TotalProfit));
            }
            catch (Exception ex)
            {
                LogErrorAndEnqueMessage(ex, ex.Message);
            }
            AppCommand.HideMessage();
        }
    }
}
