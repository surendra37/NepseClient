using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseApp.Views;

using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.MeroShare.Extensions;
using NepseClient.Modules.MeroShare.Views;
using NepseClient.Modules.TradeManagementSystem.Views;

using Newtonsoft.Json;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using Serilog;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Requests;

namespace NepseApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly TmsClient _client;
        private readonly IDialogService _dialog;
        private readonly MeroshareClient _meroshareClient;
        private readonly IConfiguration _config;
        private readonly IRegionManager _regionManager;

        public IApplicationCommand ApplicationCommand { get; }
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

        public IEnumerable<INavigationItem> NavigationItems => GetNavigationItem().ToArray();

        private object UpdateNavigationSelection(string source)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, source);
            //Settings.Default.SelectedTab = source;
            //Settings.Default.Save();
            return null;
        }

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommand applicationCommand,
            TmsClient nepse, IDialogService dialog, MeroshareClient meroshareClient, IConfiguration config)
        {
            _regionManager = regionManager;
            _client = nepse;
            _dialog = dialog;
            _meroshareClient = meroshareClient;
            _config = config;
            ApplicationCommand = applicationCommand;

            applicationCommand.ShowMessage = ShowMessage;
            _client.PromptCredentials = GetTmsCredentials;
            _meroshareClient.PromptCredential = GetMeroShareCredentials;
        }

        private IEnumerable<INavigationItem> GetNavigationItem()
        {
            var selectedTab = Settings.Default.SelectedTab;
            yield return new FirstLevelNavigationItem()
            {
                Label = "Portfolio",
                Icon = PackIconKind.Person,
                IsSelected = selectedTab.Equals(nameof(PortfolioPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(PortfolioPage))
            };

            yield return new FirstLevelNavigationItem()
            {
                Label = "My ASBA",
                Icon = PackIconKind.Web,
                IsSelected = selectedTab.Equals(nameof(AsbaPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(AsbaPage))
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Live",
                Icon = PackIconKind.XboxLive,
                IsSelected = selectedTab.Equals(nameof(TmsLiveMarketPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(TmsLiveMarketPage))
            };

            yield return new FirstLevelNavigationItem()
            {
                Label = "Live",
                Icon = PackIconKind.ArrowTop,
                IsSelected = selectedTab.Equals(nameof(TopSecuritiesPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(AsbaPage))
            };

            yield return new FirstLevelNavigationItem()
            {
                Label = "Settings",
                Icon = PackIconKind.Settings,
                IsSelected = selectedTab.Equals(nameof(SettingsPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(SettingsPage))
            };
        }

        private DelegateCommand<object> _showMessageCommand;
        public DelegateCommand<object> ShowMessageCommand =>
            _showMessageCommand ?? (_showMessageCommand = new DelegateCommand<object>(ExecuteShowMessageCommand));

        void ExecuteShowMessageCommand(object parameter)
        {
            MessageQueue.Enqueue(parameter);
        }

        private bool _isImporting;
        public bool IsImporting
        {
            get { return _isImporting; }
            set
            {
                if (SetProperty(ref _isImporting, value))
                {
                    ImportPortfolioCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DelegateCommand _importPortfolioCommand;
        public DelegateCommand ImportPortfolioCommand =>
            _importPortfolioCommand ?? (_importPortfolioCommand = new DelegateCommand(ExecuteImportPortfolioCommand, () => !IsImporting));

        void ExecuteImportPortfolioCommand()
        {
            try
            {
                IsImporting = true;
                MessageQueue.Enqueue("Importing wacc from meroshare");

                var myShares = _meroshareClient.GetMyShares();
                var waccs = _meroshareClient.GetWaccs(myShares).ToArray();

                var path = Path.Combine(PathConstants.AppDataPath.Value, "wacc.json");
                File.WriteAllText(path, JsonConvert.SerializeObject(waccs));
                MessageQueue.Enqueue("Wacc imported.");
                IsImporting = false;
            }
            catch (System.Exception ex)
            {
                IsImporting = false;
                MessageQueue.Enqueue("Failed to load portfolio. Error: " + ex.Message);
                Log.Error(ex, "Failed to import portfolio");
            }
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
    }
}
