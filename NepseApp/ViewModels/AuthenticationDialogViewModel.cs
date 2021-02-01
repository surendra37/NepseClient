using NepseClient.Commons.Extensions;
using NepseClient.Libraries.TradeManagementSystem.Models.Requests;
using NepseClient.Modules.MeroShare.Extensions;

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
        public event Action<IDialogResult> RequestClose;

        public string Title { get; set; }

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

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            try
            {
                var username = Username;
                var password = Password.GetString();
                var request = new TmsAuthenticationRequest(username, password);
                var parameters = new DialogParameters
                {
                    { "Username", username },
                    { "Password", password },
                    { "RememberPassword", IsRememberPassword },

                };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
            }
            catch (Exception ex)
            {
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
                Username = parameters.GetUsername();
                Password = parameters.GetPassword().ToSecuredString();
                IsRememberPassword = parameters.GetRememberPassword();
                Title = parameters.GetTitle();
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to get values from restored session");
            }
        }
    }
}
