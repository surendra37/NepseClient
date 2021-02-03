using System.Security;

using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly MeroshareClient _client;

        private MeroshareCapitalResponse[] _items;
        public MeroshareCapitalResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private MeroshareCapitalResponse _selectedItems;
        public MeroshareCapitalResponse SelectedItem
        {
            get { return _selectedItems; }
            set { SetProperty(ref _selectedItems, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private SecureString _securePassword;
        public SecureString SecurePassword
        {
            get { return _securePassword; }
            set { SetProperty(ref _securePassword, value); }
        }

        private bool _isRememberPassword;
        public bool IsRememberPassword
        {
            get { return _isRememberPassword; }
            set { SetProperty(ref _isRememberPassword, value); }
        }

        public LoginPageViewModel(MeroshareClient client)
        {
            _client = client;
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        void ExecuteRefreshCommand()
        {
            try
            {
                Items = _client.GetCapitals();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to get capitals");
            }
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {

        }
    }
}
