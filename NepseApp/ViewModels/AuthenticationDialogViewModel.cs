using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Serilog;
using System;
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

        private bool _isRememberPassword = true;
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

        async void ExecuteLoginCommand()
        {
            try
            {
                IsBusy = true;
                await _client.AuthenticateAsync(Host, Username, Password.GetString());
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                IsBusy = false;

                // Save values
                Settings.Default.TmsHost = Host;
                Settings.Default.TmsUsername = Username;
                Settings.Default.TmsPassword = IsRememberPassword ? Password.ToUnsecuredString() : string.Empty;
                Settings.Default.TmsRememberPassword = IsRememberPassword;
                Settings.Default.Save();

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
                Host = Settings.Default.TmsHost;
                Username = Settings.Default.TmsUsername;
                Password = Settings.Default.TmsPassword.ToSecuredString();
                IsRememberPassword = Settings.Default.TmsRememberPassword;

                //if (ConfigUtils.AutoLogin)
                //{
                //    LoginCommand.Execute();
                //}
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to get values from restored session");
            }
        }
    }
}
