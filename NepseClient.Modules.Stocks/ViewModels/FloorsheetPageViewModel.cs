using NepseClient.Commons.Contracts;
using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Stocks.Models;

using Prism.Commands;

using System;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class FloorsheetPageViewModel : PaginationBase
    {
        private readonly ServiceClient _client;

        private FloorsheetContent[] _items;
        public FloorsheetContent[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private string _buyer;
        public string Buyer
        {
            get { return _buyer; }
            set { SetProperty(ref _buyer, value); }
        }

        private string _seller;
        public string Seller
        {
            get { return _seller; }
            set { SetProperty(ref _seller, value); }
        }

        public int[] Limits { get; } = new[] { 10, 20, 50, 200, 300, 500 };

        private int _limit = 20;
        public int Limit
        {
            get { return _limit; }
            set { SetProperty(ref _limit, value); }
        }

        private double _totalAmount;
        public double TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }

        private long _totalQty;
        public long TotalQty
        {
            get { return _totalQty; }
            set { SetProperty(ref _totalQty, value); }
        }

        private long _totalTrades;
        public long TotalTrades
        {
            get { return _totalTrades; }
            set { SetProperty(ref _totalTrades, value); }
        }

        public FloorsheetPageViewModel(IApplicationCommand appCommand, ServiceClient client) : base(appCommand)
        {
            _client = client;
            RefreshCommand.Execute();
        }

        protected override void Navigate(int page)
        {
            try
            {
                try
                {
                    IsBusy = true;
                    var items = _client.GetFloorsheet(page - 1, Buyer, Seller, Limit);
                    Items = items.Floorsheets.Content;
                    TotalPage = items.Floorsheets.TotalPages;
                    CurrentPage = items.Floorsheets.Number + 1;
                    TotalAmount = items.TotalAmount;
                    TotalQty = items.TotalQty;
                    TotalTrades = items.TotalTrades;
                    IsBusy = false;

                    base.Navigate(page);
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    LogErrorAndEnqueMessage(ex, "Failed to get news");
                }
            }
            catch (Exception ex)
            {
                LogErrorAndEnqueMessage(ex, "Failed to navigate");
            }
        }

        private DelegateCommand _filterCommand;
        public DelegateCommand FilterCommand =>
            _filterCommand ?? (_filterCommand = new DelegateCommand(ExecuteFilterCommand));

        void ExecuteFilterCommand()
        {
            RefreshCommand.Execute();
        }

        private DelegateCommand _resetFilterCommand;
        public DelegateCommand ResetFilterCommand =>
            _resetFilterCommand ?? (_resetFilterCommand = new DelegateCommand(ExecuteResetFilterCommand));

        void ExecuteResetFilterCommand()
        {
            Buyer = null;
            Seller = null;
            Limit = 20;
            RefreshCommand.Execute();
        }
    }
}
