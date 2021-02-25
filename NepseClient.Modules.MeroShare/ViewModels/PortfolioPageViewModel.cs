using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class PortfolioPageViewModel : PaginationBase
    {
        private readonly int _size = 200;
        private readonly MeroshareClient _client;

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

        public PortfolioPageViewModel(MeroshareClient client, IApplicationCommand applicationCommand) :
            base(applicationCommand)
        {
            _client = client;
        }

        protected override async void Navigate(int page)
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Loading portfolio...");
                var portfolios = await _client.GetMyPortfoliosAsync(page, _size);
                if (portfolios is not null)
                {
                    Items = portfolios?.MeroShareMyPortfolio;
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
        }
    }
}
