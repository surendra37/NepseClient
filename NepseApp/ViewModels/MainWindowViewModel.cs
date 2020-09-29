using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using NepseApp.Models;
using NepseApp.Views;
using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Windows;

namespace NepseApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IAuthenticatableNepseClient _client;
        private readonly IDialogService _dialog;
        private readonly IRegionManager _regionManager;

        public IApplicationCommand ApplicationCommand { get; }
        public ISnackbarMessageQueue MessageQueue => ApplicationCommand.MessageQueue;

        private string _title = "Nepse App";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public bool IsAuthenticated
        {
            get
            {
                try
                {
                    return _client.IsAuthenticated;
                }
                catch 
                {
                    return false;
                }
            }
        }

        public IEnumerable<INavigationItem> NavigationItems => GetNavigationItem().ToArray();

        private object UpdateNavigationSelection(string source)
        {
            _regionManager.RequestNavigate("ContentRegion", source);
            return null;
        }

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommand applicationCommand,
            IAuthenticatableNepseClient nepse, IDialogService dialog)
        {
            _regionManager = regionManager;
            _client = nepse;
            _dialog = dialog;
            ApplicationCommand = applicationCommand;
            //_client.ShowAuthenticationDialog = ExecuteLoginCommand;
            _client.ShowAuthenticationDialog = () => Application.Current.Dispatcher.Invoke(ExecuteLoginCommand);
        }

        private IEnumerable<INavigationItem> GetNavigationItem()
        {
            yield return new SubheaderNavigationItem() { Subheader = "General" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Dashboard",
                Icon = PackIconKind.ViewDashboard,
                IsSelected = true,
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(DashboardPage)),
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Portfolio",
                Icon = PackIconKind.Person,
                IsSelected = false,
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(PortfolioPage))
            };
            yield return new DividerNavigationItem();

            yield return new SubheaderNavigationItem() { Subheader = "Market" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Live",
                Icon = PackIconKind.RecordCircle,
                IsSelected = false,
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(LiveMarketPage)),
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Settings",
                Icon = PackIconKind.Settings,
                IsSelectable = false,
            };
        }

        private DelegateCommand<object> _showMessageCommand;
        public DelegateCommand<object> ShowMessageCommand =>
            _showMessageCommand ?? (_showMessageCommand = new DelegateCommand<object>(ExecuteShowMessageCommand));

        void ExecuteShowMessageCommand(object parameter)
        {
            MessageQueue.Enqueue(parameter);
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            try
            {
                var success = false;
                _dialog.ShowDialog(nameof(AuthenticationDialog), new DialogParameters { { "Client", _client } }, result =>
                {
                    success = result?.Result == ButtonResult.OK;
                });
                if (!success)
                    throw new AuthenticationException("Not Authenticated.");
                MessageQueue.Enqueue("Authentication successful");
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to authenticate");
                MessageQueue.Enqueue("Authenticate failed/cancelled.");
            }

            RaisePropertyChanged(nameof(IsAuthenticated));
        }

        private DelegateCommand _logoutCommand;
        public DelegateCommand LogoutCommand =>
            _logoutCommand ?? (_logoutCommand = new DelegateCommand(ExecuteLogoutCommand));

        void ExecuteLogoutCommand()
        {
            try
            {
                var res = MessageBox.Show("Do you want to log out?", "Log out", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    // Log out
                    _client.Logout();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to logout");
                MessageQueue.Enqueue("Failed to logout");
            }

            RaisePropertyChanged(nameof(IsAuthenticated));
        }
    }
}
