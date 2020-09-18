using NepseClient.Commons;
using NepseClient.Commons.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;

namespace NepseApp.ViewModels
{
    public class AuthenticationDialogViewModel : BindableBase, IDialogAware
    {
        private INepseClient _client;
        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = "Authenticate";

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

        private bool _isRememberPassword;
        public bool IsRememberPassword
        {
            get { return _isRememberPassword; }
            set { SetProperty(ref _isRememberPassword, value); }
        }

        public AuthenticationDialogViewModel()
        {

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
                _client.Authenticate(Host, Username, Password.GetString());
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                IsBusy = false;

                // Save values
                ConfigUtils.TmsHost = Host;
                ConfigUtils.TmsUsername = Username;
                ConfigUtils.TmsPassword = IsRememberPassword ? Password.ToUnsecuredString() : string.Empty;
                ConfigUtils.SaveSettings();

            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Error(ex, "Failed to login");
                MessageBox.Show(ex.Message, "Failed to login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { _client = null; }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _client = parameters.GetValue<INepseClient>("Client");
            try
            {
                Host = ConfigUtils.TmsHost;
                Username = ConfigUtils.TmsUsername;
                Password = ConfigUtils.TmsPassword.ToSecuredString();
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to get values from restored session");
            }
        }
    }
}
