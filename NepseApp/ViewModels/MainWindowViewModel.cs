using MaterialDesignThemes.Wpf;

using NepseApp.Views;

using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;
using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Requests;
using NepseClient.Libraries.NepalStockExchange.Contexts;
using NepseClient.Libraries.TradeManagementSystem;
using NepseClient.Libraries.TradeManagementSystem.Requests;
using NepseClient.Modules.Commons.Events;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Extensions;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using Serilog;

using System;

namespace NepseApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly TmsClient _client;
        private readonly IDialogService _dialog;
        private readonly DatabaseContext _database;
        private readonly MeroshareClient _meroshareClient;
        private readonly IConfiguration _config;
        private readonly IRegionManager _regionManager;

        public IApplicationCommand ApplicationCommand { get; }

        private UpdateUIEvent _updateUIEvent;

        public ISnackbarMessageQueue MessageQueue => ApplicationCommand.MessageQueue;

        private string _title = "Nepse App";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public bool ShowNotices
        {
            get => _config.ShowNepseNotice;
            set
            {
                _config.ShowNepseNotice = value;
                _updateUIEvent.Publish(nameof(RegionNames.SideNavRegion));
            }
        }
        public bool ShowFloorsheets
        {
            get => _config.ShowFloorsheet;
            set
            {
                _config.ShowFloorsheet = value;
                _updateUIEvent.Publish(nameof(RegionNames.SideNavRegion));
            }
        }

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommand applicationCommand,
            TmsClient nepse, IDialogService dialog, IEventAggregator events, DatabaseContext database,
            MeroshareClient meroshareClient, IConfiguration config)
        {
            _regionManager = regionManager;
            _client = nepse;
            _dialog = dialog;
            _database = database;
            _meroshareClient = meroshareClient;
            _config = config;
            ApplicationCommand = applicationCommand;

            _updateUIEvent = events.GetEvent<UpdateUIEvent>();

            applicationCommand.ShowMessage = ShowMessage;
            _meroshareClient.PromptCredential = GetMeroShareCredentials;
        }

        private DelegateCommand<object> _showMessageCommand;
        public DelegateCommand<object> ShowMessageCommand =>
            _showMessageCommand ?? (_showMessageCommand = new DelegateCommand<object>(ExecuteShowMessageCommand));

        void ExecuteShowMessageCommand(object parameter)
        {
            MessageQueue.Enqueue(parameter);
        }

        private void ShowMessage(string message)
        {
            Message = message;
        }

        private TmsAuthenticationRequest GetTmsCredentials()
        {
            var config = _config.Tms;
            var parameters = new DialogParameters()
                .AddTitle("Authorize TMS")
                .AddUsername(config.Username)
                .AddPassword(config.Password)
                .AddRememberPassword(config.RememberPassword);
            var success = false;
            _dialog.ShowDialog(nameof(AuthenticationDialog), parameters,
                result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        var p = result.Parameters;
                        config.Username = p.GetUsername();
                        config.Password = p.GetPassword();
                        config.RememberPassword = p.GetRememberPassword();
                        config.Save();
                        success = true;
                    }
                });

            if (!success) return null;

            return new TmsAuthenticationRequest(config.Username, config.Password);
        }

        private MeroshareAuthRequest GetMeroShareCredentials()
        {
            var config = _config.Meroshare;
            var parameters = new DialogParameters()
                .AddTitle("Authorize MeroShare")
                .AddUsername(config.Username)
                .AddPassword(config.Password)
                .AddRememberPassword(config.RememberPassword);
            var success = false;
            _dialog.ShowDialog(nameof(AuthenticationDialog), parameters,
                result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        var p = result.Parameters;
                        config.Username = p.GetUsername();
                        config.Password = p.GetPassword();
                        config.RememberPassword = p.GetRememberPassword();
                        config.Save();
                        success = true;
                    }
                });

            if (!success) return null;

            return new MeroshareAuthRequest(config.ClientId, config.Username, config.Password);
        }

        #region Tms
        private bool _isTmsLoggedIn;
        public bool IsTmsLoggedIn
        {
            get { return _isTmsLoggedIn; }
            set { SetProperty(ref _isTmsLoggedIn, value); }
        }

        private DelegateCommand _updateTmsBaseUrl;
        public DelegateCommand UpdateTmsBaseUrl =>
            _updateTmsBaseUrl ?? (_updateTmsBaseUrl = new DelegateCommand(ExecuteUpdateTmsBaseUrl));

        void ExecuteUpdateTmsBaseUrl()
        {
            try
            {
                var baseUrl = _config.Tms.BaseUrl;
                var dialog = new Ookii.Dialogs.WinForms.InputDialog
                {
                    WindowTitle = "Update TMS Base URL",
                    MainInstruction = "Please provide the default base url provided by your broker",
                    Content = "Enter the base URL eg: https://tmsXX.nepsetms.com.np, to use for connecting to the TMS service",
                    Input = baseUrl,
                };
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _config.Tms.BaseUrl = dialog.Input;
                    _config.Tms.Save();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update tms base url");
                MessageBoxManager.ShowErrorMessage(ex, "Update Failed");
            }
        }

        private DelegateCommand _tmsLogInCommand;
        public DelegateCommand TmsLogInCommand =>
            _tmsLogInCommand ?? (_tmsLogInCommand = new DelegateCommand(ExecuteTmsLogInCommand));

        async void ExecuteTmsLogInCommand()
        {
            try
            {
                await _client.SignInAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log in to tms");
                MessageBoxManager.ShowErrorMessage(ex, "Login Failed");
            }

            IsTmsLoggedIn = _client.IsAuthenticated;
        }

        private DelegateCommand _tmsLogoutCommand;
        public DelegateCommand TmsLogOutCommand =>
            _tmsLogoutCommand ?? (_tmsLogoutCommand = new DelegateCommand(ExecuteTmsLogOutCommand));

        async void ExecuteTmsLogOutCommand()
        {
            try
            {
                await _client.SignOutAsync();
                var url = _config.Tms.BaseUrl;
                IsTmsLoggedIn = false;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log out to tms");
                MessageBoxManager.ShowErrorMessage(ex, "Log out Failed");
            }

            IsTmsLoggedIn = _client.IsAuthenticated;
        }
        #endregion

        #region MeroShare
        private bool _isMeroShareLoggedIn;
        public bool IsMeroShareLoggedIn
        {
            get { return _isMeroShareLoggedIn; }
            set { SetProperty(ref _isMeroShareLoggedIn, value); }
        }

        private DelegateCommand _updateMeroShareClientId;
        public DelegateCommand UpdateMeroShareClientId =>
            _updateMeroShareClientId ?? (_updateMeroShareClientId = new DelegateCommand(ExecuteUpdateMeroShareClientId));

        void ExecuteUpdateMeroShareClientId()
        {
            _dialog.ShowDialog(nameof(NepseClient.Modules.MeroShare.Views.UpdateClientDialog));
        }

        private DelegateCommand _meroShareLogInCommand;
        public DelegateCommand MeroShareLogInCommand =>
            _meroShareLogInCommand ?? (_meroShareLogInCommand = new DelegateCommand(ExecuteMeroShareLoginCommand));

        async void ExecuteMeroShareLoginCommand()
        {
            try
            {
                await _meroshareClient.SignInAsync();
                _regionManager.RequestNavigate(RegionNames.SideNavRegion, "MeroShareNavPage");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log in to tms");
                MessageBoxManager.ShowErrorMessage(ex, "Login Failed");
            }
            IsMeroShareLoggedIn = _meroshareClient.IsAuthenticated;
        }

        private DelegateCommand _meroShareLogoutCommand;
        public DelegateCommand MeroShareLogOutCommand =>
            _meroShareLogoutCommand ?? (_meroShareLogoutCommand = new DelegateCommand(ExecuteMeroShareLogOutCommand));

        async void ExecuteMeroShareLogOutCommand()
        {
            try
            {
                await _meroshareClient.SignOutAsync();
                var url = _config.Meroshare.BaseUrl;
                IsMeroShareLoggedIn = _meroshareClient.IsAuthenticated;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to log out to mero share");
                MessageBoxManager.ShowErrorMessage(ex, "Log out Failed");
            }
        }

        private DelegateCommand _importPortfolioCommand;
        public DelegateCommand ImportPortfolioCommand =>
            _importPortfolioCommand ?? (_importPortfolioCommand = new DelegateCommand(ExecuteImportPortfolioCommand));

        async void ExecuteImportPortfolioCommand()
        {
            try
            {
                var portfolio = await _meroshareClient.GetMySharesAsync();
                foreach (var item in portfolio)
                {
                    _database.Watchlist.Update(item, true);
                }
                ApplicationCommand.RefreshCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to import portfolio");
            }
        }
        #endregion

        #region Views
        private DelegateCommand<string> _updateSideNavViewCommand;
        public DelegateCommand<string> UpdateSideNavViewCommand =>
            _updateSideNavViewCommand ?? (_updateSideNavViewCommand = new DelegateCommand<string>(ExecuteUpdateSideNavViewCommand));

        void ExecuteUpdateSideNavViewCommand(string arg)
        {
            _regionManager.RequestNavigate(RegionNames.SideNavRegion, arg);
        }
        #endregion
    }
}
