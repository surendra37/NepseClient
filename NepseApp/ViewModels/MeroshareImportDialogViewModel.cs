using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using TradeManagementSystemClient;

namespace NepseApp.ViewModels
{
    public class MeroshareImportDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        public string Title { get; } = "Authenticate";

        private IEnumerable<IMeroshareCapital> _items;
        public IEnumerable<IMeroshareCapital> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private IMeroshareCapital _selectedItem;
        public IMeroshareCapital SelectedItem
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

        public MeroshareImportDialogViewModel(MeroshareClient client)
        {
            _client = client;
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

        async void ExecuteLoginCommand()
        {
            try
            {
                if (SelectedItem == null) return;

                IsBusy = true;
                var client = SelectedItem; //vVy.$3pz7wx#y9S  "150394"
                var password = Password.GetString();
                await _client.AuthenticateAsync(client.Id, Username, password);
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                IsBusy = false;

                // Save values
                Settings.Default.MeroshareClientId = SelectedItem.Id;
                Settings.Default.MeroshareUsername = Username;
                Settings.Default.MerosharePassword = IsRememberPassword ? Password.ToUnsecuredString() : string.Empty;
                Settings.Default.MeroshareRememberPassword = IsRememberPassword;
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

        public void OnDialogClosed() { }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                if (Items == null)
                {
                    Items = await _client.GetCapitalsAsync();
                }
                SelectedItem = Items.FirstOrDefault(x => x.Id == Settings.Default.MeroshareClientId);
                Username = Settings.Default.MeroshareUsername;
                Password = Settings.Default.MerosharePassword.ToSecuredString();
                IsRememberPassword = Settings.Default.MeroshareRememberPassword;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to get values from restored session");
            }
        }
    }
}
