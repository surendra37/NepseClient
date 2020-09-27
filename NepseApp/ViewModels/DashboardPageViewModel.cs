using NepseApp.Models;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace NepseApp.ViewModels
{
    public class DashboardPageViewModel : BindableBase, IActiveAware
    {
        public IApplicationCommand ApplicationCommand { get; }
        public DashboardPageViewModel(IApplicationCommand applicationCommand)
        {
            ApplicationCommand = applicationCommand;

            applicationCommand.RefreshCommand.RegisterCommand(RefreshCommand);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RefreshCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand, () => !IsBusy));

        void ExecuteRefreshCommand()
        {
        }

        #region IActiveAware
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        public event EventHandler IsActiveChanged;

        private void OnIsActiveChanged()
        {
            RefreshCommand.IsActive = IsActive; //set the command as active            
            IsActiveChanged?.Invoke(this, new EventArgs()); //invoke the event for all listeners
        }
        #endregion
    }
}
