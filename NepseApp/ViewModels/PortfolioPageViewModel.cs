using NepseClient.Commons;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NepseApp.ViewModels
{
    public class PortfolioPageViewModel : BindableBase
    {
        private readonly INepseClient _client;

        private IEnumerable<IScripResponse> _items;
        public IEnumerable<IScripResponse> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public PortfolioPageViewModel(INepseClient client)
        {
            _client = client;
            RefreshCommand.Execute();
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        void ExecuteRefreshCommand()
        {
            try
            {
                Log.Debug("Refreshing portfolio");
                //_client.Authenticate("SS482167", "Pass!@#$5word");
                //_client.Logout();

                Items = _client.GetMyPortfolio();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to refresh portfolio");
            }
        }

        private DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand =>
            _logoutCommand ?? (_logoutCommand = new DelegateCommand(ExecuteLogoutCommand));

        void ExecuteLogoutCommand()
        {
            try
            {
                _client.Logout();
                MessageBox.Show("You have been logged out", "Logged out");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to logout");
            }
        }
    }
}
