using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Stocks.Models;

using Prism.Commands;
using Prism.Regions;

using System;
using System.Diagnostics;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class NewsAndAlertPageViewModel : PaginationBase, INavigationAware
    {
        private readonly ServiceClient _service;

        private NoticeContent[] _items;
        public NoticeContent[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public NewsAndAlertPageViewModel(ServiceClient service, IApplicationCommand applicationCommand)
            : base(applicationCommand)
        {
            _service = service;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //RefreshCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }        

        protected override void Navigate(int page)
        {

            try
            {
                IsBusy = true;
                var notices = _service.GetNotices(page - 1);
                foreach (var item in notices.Content)
                {
                    item.OpenAttachmentCommand = OpenAttachmentCommand;
                }
                Items = notices.Content;
                TotalPage = notices.TotalPages;
                CurrentPage = notices.Number + 1;
                IsBusy = false;

                base.Navigate(page);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                LogErrorAndEnqueMessage(ex, "Failed to get news");
            }
        }

        private DelegateCommand<NoticeContent> _openAttachmentCommand;
        public DelegateCommand<NoticeContent> OpenAttachmentCommand =>
            _openAttachmentCommand ?? (_openAttachmentCommand = new DelegateCommand<NoticeContent>(ExecuteOpenAttachmentCommand));

        void ExecuteOpenAttachmentCommand(NoticeContent parameter)
        {
            if (parameter is null || !parameter.HasAttachment) return;

            Process.Start(new ProcessStartInfo(parameter.NoticeFileFullPath) { UseShellExecute = true });
        }
    }
}
