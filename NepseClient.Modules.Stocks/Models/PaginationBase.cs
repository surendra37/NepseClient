using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;

using System;

namespace NepseClient.Modules.Stocks.Models
{
    public abstract class PaginationBase : ActiveAwareBindableBase
    {
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

        public PaginationBase(IApplicationCommand appCommand) : base(appCommand)
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

        public override void ExecuteRefreshCommand()
        {
            Navigate(CurrentPage);
        }

        protected virtual void Navigate(int page)
        {
            FirstCommand.RaiseCanExecuteChanged();
            PreviousCommand.RaiseCanExecuteChanged();
            NextCommand.RaiseCanExecuteChanged();
            LastCommand.RaiseCanExecuteChanged();
        }
    }
}
