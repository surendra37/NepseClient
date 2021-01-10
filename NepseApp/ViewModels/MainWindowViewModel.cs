using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseApp.Models;
using NepseApp.Views;

using NepseClient.Commons;

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
            _regionManager.RequestNavigate("ContentRegion", source);
            Settings.Default.SelectedTab = source;
            Settings.Default.Save();
            return null;
        }

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommand applicationCommand,
            TmsClient nepse, IDialogService dialog, MeroshareClient meroshareClient)
        {
            _regionManager = regionManager;
            _client = nepse;
            _dialog = dialog;
            _meroshareClient = meroshareClient;
            ApplicationCommand = applicationCommand;

            applicationCommand.ShowMessage = ShowMessage;
            _client.PromptCredentials = GetTmsCredentials;
            _meroshareClient.PromptCredential = GetMeroShareCredentials;
        }

        private IEnumerable<INavigationItem> GetNavigationItem()
        {
            yield return new FirstLevelNavigationItem()
            {
                Label = "Portfolio",
                Icon = PackIconKind.Person,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(PortfolioPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(PortfolioPage))
            };

            yield return new FirstLevelNavigationItem()
            {
                Label = "My ASBA",
                Icon = PackIconKind.Web,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(MeroShareAsbaPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(MeroShareAsbaPage))
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Live Market",
                Icon = PackIconKind.XboxLive,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(TmsLiveMarketPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(TmsLiveMarketPage))
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

                var path = Path.Combine(Constants.AppDataPath.Value, "wacc.json");
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

        private AuthenticationRequest GetTmsCredentials()
        {
            AuthenticationRequest output = null;
            _dialog.ShowDialog(nameof(AuthenticationDialog), null, result =>
                {
                    if(result?.Result == ButtonResult.OK)
                    {
                        result.Parameters.TryGetValue("Credentials", out output);
                    }
                    //success = result?.Result == ButtonResult.OK;
                });
            return output;
        }

        private MeroshareAuthRequest GetMeroShareCredentials()
        {
            MeroshareAuthRequest output = null;
            _dialog.ShowDialog(nameof(MeroshareImportDialog), null,
                result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        result.Parameters.TryGetValue("Credentials", out output);
                    }
                });
            return output;
        }
    }
}
