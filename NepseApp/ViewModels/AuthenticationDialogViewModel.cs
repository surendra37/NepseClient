
using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using Serilog;

using System;
using System.Security;
using System.Windows;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Requests;

namespace NepseApp.ViewModels
{
    public class AuthenticationDialogViewModel : BindableBase, IDialogAware
    {
        private readonly ITmsConfiguration _config;
        private readonly TmsClient _client;
        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = "TMS Authentication";

        private string _host;
        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
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

        public AuthenticationDialogViewModel(IConfiguration config)
        {
            _config = config.Tms;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { if (SetProperty(ref _isBusy, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand, () => !IsBusy));

        void ExecuteLoginCommand()
        {
            try
            {
                IsBusy = true;
                var url = Host;
                var username = Username;
                var password = Password.GetString();
                var request = new AuthenticationRequest(username, password) { BaseUrl = new Uri(url) };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "Credentials", request } }));
                IsBusy = false;

                // Save values
                _config.BaseUrl = url;
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
                Host = _config.BaseUrl;
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
