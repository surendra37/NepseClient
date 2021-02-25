using NepseClient.Commons.Contracts;
using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using Serilog;

using System;
using System.Linq;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class UpdateClientDialogViewModel : BindableBase, IDialogAware
    {
        private MeroshareCapitalResponse[] _items;
        public MeroshareCapitalResponse[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        private MeroshareCapitalResponse _selected;
        public MeroshareCapitalResponse SelectedItem
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        public UpdateClientDialogViewModel(MeroshareClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
            RefreshCommand.Execute();
        }

        public string Title => "Update Client";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        private DelegateCommand _acceptCommand;
        public DelegateCommand AcceptCommand =>
            _acceptCommand ?? (_acceptCommand = new DelegateCommand(ExecuteAcceptCommand));

        void ExecuteAcceptCommand()
        {
            try
            {
                var id = SelectedItem?.Id;
                _config.Meroshare.ClientId = id;
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to accept");
            }
        }

        private DelegateCommand _cancelCommand;
        private readonly MeroshareClient _client;
        private readonly IConfiguration _config;

        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        async void ExecuteRefreshCommand()
        {
            try
            {
                Items = await _client.GetCapitalsAsync();
                var id = _config.Meroshare.ClientId;
                SelectedItem = Items?.FirstOrDefault(x => x.Id.Equals(id) || string.IsNullOrEmpty(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to dps");
            }
        }
    }
}
