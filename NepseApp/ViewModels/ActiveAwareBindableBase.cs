using NepseApp.Models;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;

namespace NepseApp.ViewModels
{
    public abstract class ActiveAwareBindableBase : BindableBase, IActiveAware
    {
        public IApplicationCommand AppCommand { get; }
        public ActiveAwareBindableBase(IApplicationCommand appCommand)
        {
            AppCommand = appCommand;
            AppCommand.RefreshCommand.RegisterCommand(RefreshCommand);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnIsBusyUpdated();
                }
            }
        }

        protected virtual void OnIsBusyUpdated()
        {
            RefreshCommand.RaiseCanExecuteChanged();
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        public virtual void ExecuteRefreshCommand() { }

        protected void EnqueMessage(object message)
        {
            AppCommand.MessageQueue.Enqueue(message);
        }

        protected void LogDebugAndEnqueMessage(string message)
        {
            Log.Debug(message);
            AppCommand.MessageQueue.Enqueue(message);
        }

        protected void LogErrorAndEnqueMessage(Exception ex, string message)
        {
            Log.Error(ex, message);
            AppCommand.MessageQueue.Enqueue(message);
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

        protected virtual void OnIsActiveChanged()
        {
            RefreshCommand.IsActive = IsActive; //set the command as active
            IsActiveChanged?.Invoke(this, new EventArgs()); //invoke the event for all listeners

            if(IsActive && AppCommand.RefreshOnActive)
            {
                RefreshCommand.Execute();
            }
        }
        #endregion
    }
}
