using NepseApp.Models;
using NepseApp.Utils;

using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;

namespace NepseApp.ViewModels
{
    public class MeroshareImportDialogViewModel : BindableBase, IDialogAware
    {
        private readonly IMeroShareConfiguration _config;
        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = "Meroshare Authentication";

        private IEnumerable<MeroshareCapitalResponse> _items;
        public IEnumerable<MeroshareCapitalResponse> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private MeroshareCapitalResponse _selectedItem;
        public MeroshareCapitalResponse SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private SecureString _password;
        public SecureString Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private bool _isRememberPassword = true;
        public bool IsRememberPassword
        {
            get { return _isRememberPassword; }
            set { SetProperty(ref _isRememberPassword, value); }
        }

        public MeroshareImportDialogViewModel(MeroshareClient client, IConfiguration config)
        {
            _client = client;
            _config = config.Meroshare;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { if (SetProperty(ref _isBusy, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }

        private DelegateCommand _loginCommand;
        private readonly MeroshareClient _client;

        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand, () => !IsBusy));

        void ExecuteLoginCommand()
        {
            try
            {
                if (SelectedItem == null) return;

                IsBusy = true;
                var client = SelectedItem; //vVy.$3pz7wx#y9S  "150394"
                var clientId = SelectedItem.Id;
                var username = Username;
                var password = Password.GetString();
                var request = new MeroshareAuthRequest(clientId, username, password);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "Credentials", request } }));
                IsBusy = false;

                // Save values
                _config.ClientId = clientId;
                _config.Username = username;
                _config.Password = IsRememberPassword ? password : string.Empty;
                _config.RememberPassword = IsRememberPassword;
                _config.Save();

            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Error(ex, "Failed to login");
                MessageBox.Show(ex.Message, "Failed to login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                if (Items == null)
                {
                    Items = _client.GetCapitals();
                }
                SelectedItem = Items.FirstOrDefault(x => x.Id == _config.ClientId);
                Username = _config.Username;
                Password = _config.Password.ToSecuredString();
                IsRememberPassword = _config.RememberPassword;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to get values from restored session");
            }
        }
    }
}
