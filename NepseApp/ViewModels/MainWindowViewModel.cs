﻿using MaterialDesignExtensions.Model;

using MaterialDesignThemes.Wpf;

using NepseApp.Models;
using NepseApp.Views;

using NepseClient.Commons;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

using Serilog;

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Windows;

using TradeManagementSystemClient;

namespace NepseApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly INepseClient _client;
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
            Settings.Default.SelectedTab = source;
            Settings.Default.Save();
            return null;
        }

        public MainWindowViewModel(IRegionManager regionManager, IApplicationCommand applicationCommand,
            INepseClient nepse, IDialogService dialog, MeroshareClient meroshareClient)
        {
            _regionManager = regionManager;
            _client = nepse;
            _dialog = dialog;
            _meroshareClient = meroshareClient;
            ApplicationCommand = applicationCommand;
            _client.ShowAuthenticationDialog = ExecuteLoginCommand;
            applicationCommand.ShowMessage = ShowMessage;
        }

        private IEnumerable<INavigationItem> GetNavigationItem()
        {

            //yield return new SubheaderNavigationItem() { Subheader = "General" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Dashboard",
                Icon = PackIconKind.ViewDashboard,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(DashboardPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(DashboardPage)),
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Portfolio",
                Icon = PackIconKind.Person,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(PortfolioPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(PortfolioPage))
            };
            //yield return new DividerNavigationItem();

            //yield return new SubheaderNavigationItem() { Subheader = "Market" };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Live",
                Icon = PackIconKind.RecordCircle,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(LiveMarketPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(LiveMarketPage)),
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Market Depth",
                Icon = PackIconKind.Eye,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(MarketDepthPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(MarketDepthPage)),
            };
            yield return new FirstLevelNavigationItem()
            {
                Label = "Settings",
                Icon = PackIconKind.Settings,
                IsSelected = Settings.Default.SelectedTab.Equals(nameof(SettingsPage)),
                NavigationItemSelectedCallback = _ => UpdateNavigationSelection(nameof(SettingsPage)),
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

        async void ExecuteImportPortfolioCommand()
        {
            try
            {
                IsImporting = true;
                MessageQueue.Enqueue("Importing wacc from meroshare");

                var me = await _meroshareClient.GetOwnDetailsAsync();
                var myShares = await _meroshareClient.GetMySharesAsync();
                if (!int.TryParse(ConfigurationManager.AppSettings["max_retry"], out int maxRetry))
                {
                    maxRetry = 3;
                }

                using (var viewFile = new StreamWriter(Path.Combine(Constants.AppDataPath.Value, "view.jl.bk")))
                using (var searchFile = new StreamWriter(Path.Combine(Constants.AppDataPath.Value, "search.jl.bk")))
                    foreach (var share in myShares)
                    {
                        var retry = 0;
                        while (true)
                        {
                            try
                            {
                                var view = await _meroshareClient.ViewMyPurchaseAsync(me.Demat, share);
                                await viewFile.WriteLineAsync(JsonConvert.SerializeObject(view));
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                retry++;
                                Log.Warning(ex, "Failed to load view of {share}. Retrying {retry} times", share, retry);
                            }

                            if (retry > maxRetry)
                            {
                                Log.Error("Exceed max retries({retry}).", retry);
                                break;
                            }
                        }

                        retry = 0;
                        while (true)
                        {
                            try
                            {
                                var searches = await _meroshareClient.SearchMyPurchaseAsync(me.Demat, share);
                                await searchFile.WriteLineAsync(JsonConvert.SerializeObject(searches));
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                retry++;
                                Log.Error(ex, "Failed to load search of {share}. Retrying {retry} times", share, retry);
                            }
                            if (retry > maxRetry)
                            {
                                Log.Error("Exceed max retries({retry}).", retry);
                                break;
                            }
                        }
                    }

                // Replace the old file
                File.Move(Path.Combine(Constants.AppDataPath.Value, "view.jl.bk"), Path.Combine(Constants.AppDataPath.Value, "view.jl"), true);
                File.Move(Path.Combine(Constants.AppDataPath.Value, "search.jl.bk"), Path.Combine(Constants.AppDataPath.Value, "search.jl"), true);

                _client.LoadWacc();
                MessageQueue.Enqueue("Wacc imported.");
                IsImporting = false;
            }
            catch (AuthenticationException)
            {
                IsImporting = false;
                _dialog.ShowDialog(nameof(MeroshareImportDialog), null, result =>
                {
                    if (result?.Result == ButtonResult.OK)
                    {
                        ImportPortfolioCommand.Execute();
                    }
                });
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
    }
}
