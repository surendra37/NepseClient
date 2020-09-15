using NepseClient.Commons;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.Generic;

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
    }
}
