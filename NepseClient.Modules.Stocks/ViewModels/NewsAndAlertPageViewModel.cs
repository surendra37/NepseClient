using NepseClient.Libraries.NepalStockExchange;
using NepseClient.Libraries.NepalStockExchange.Responses;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;
using Prism.Regions;

using System;
using System.Diagnostics;

namespace NepseClient.Modules.Stocks.ViewModels
{
    public class NewsAndAlertPageViewModel : ActiveAwareBindableBase, INavigationAware
    {
        private readonly ServiceClient _service;

        private NoticeContent[] _items;
        public NoticeContent[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { SetProperty(ref _currentPage, value); }
        }

        private int _totalPage = 1;
        public int TotalPage
        {
            get { return _totalPage; }
            set { SetProperty(ref _totalPage, value); }
        }

        public NewsAndAlertPageViewModel(ServiceClient service, IApplicationCommand applicationCommand)
            : base(applicationCommand)
        {
            _service = service;
        }

        public override void ExecuteRefreshCommand()
        {
            Navigate(CurrentPage);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RefreshCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        private DelegateCommand _firstCommand;
        public DelegateCommand FirstCommand =>
            _firstCommand ?? (_firstCommand = new DelegateCommand(ExecuteFirstCommand, CanExecutePreviousCommand));

        void ExecuteFirstCommand()
        {
            Navigate(1);
        }

        private DelegateCommand _previousCommand;
        public DelegateCommand PreviousCommand =>
            _previousCommand ?? (_previousCommand = new DelegateCommand(ExecutePreviousCommand, CanExecutePreviousCommand));

        void ExecutePreviousCommand()
        {
            Navigate(CurrentPage - 1);
        }

        bool CanExecutePreviousCommand()
        {
            return CurrentPage > 1;
        }

        private DelegateCommand _nextCommand;
        public DelegateCommand NextCommand =>
            _nextCommand ?? (_nextCommand = new DelegateCommand(ExecuteNextCommand, CanExecuteNextCommand));

        void ExecuteNextCommand()
        {
            Navigate(CurrentPage + 1);
        }

        bool CanExecuteNextCommand()
        {
            return CurrentPage < TotalPage;
        }

        private DelegateCommand _lastCommand;
        public DelegateCommand LastCommand =>
            _lastCommand ?? (_lastCommand = new DelegateCommand(ExecuteLastCommand, CanExecuteNextCommand));

        void ExecuteLastCommand()
        {
            Navigate(TotalPage);
        }

        private void Navigate(int page)
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

                FirstCommand.RaiseCanExecuteChanged();
                PreviousCommand.RaiseCanExecuteChanged();
                NextCommand.RaiseCanExecuteChanged();
                LastCommand.RaiseCanExecuteChanged();
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
