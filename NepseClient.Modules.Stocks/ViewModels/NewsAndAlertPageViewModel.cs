using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class NewsAndAlertPageViewModel : ActiveAwareBindableBase, INavigationAware
    {
        private readonly ServiceClient _service;

        private NewsAndAlertResponse[] _items;
        public NewsAndAlertResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public NewsAndAlertPageViewModel(ServiceClient service, IApplicationCommand applicationCommand)
            : base(applicationCommand)
        {
            _service = service;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                Items = _service.GetNews();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to get news");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
