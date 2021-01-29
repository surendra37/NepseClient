using System;

using NepseApp.Views;

using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;

using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareApplyForIssuePageViewModel : ActiveAwareBindableBase, ITabPage, IConfirmNavigationRequest
    {
        private readonly MeroshareClient _client;
        private readonly IDialogService _dialog;

        public string Title { get; } = "Apply For Issue";

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareApplyForIssuePageViewModel(IApplicationCommand appCommand,
            MeroshareClient client, IDialogService dialog) : base(appCommand)
        {
            _client = client;
            _dialog = dialog;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                AppCommand.ShowMessage("Getting applicable issues");
                IsBusy = true;
                Items = _client.GetApplicableIssues().Object;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Debug(ex, "Failed to get applicable issues");
                EnqueMessage("Failed to get applicable issues");
            }
            AppCommand.HideMessage();
        }

        private DelegateCommand<ApplicationReportItem> _applyCommand;
        public DelegateCommand<ApplicationReportItem> ApplyCommand =>
            _applyCommand ?? (_applyCommand = new DelegateCommand<ApplicationReportItem>(ExecuteApplyCommand));

        void ExecuteApplyCommand(ApplicationReportItem item)
        {
            if (item is null) return;

            var isApply = item.Action.Equals("apply", StringComparison.OrdinalIgnoreCase);

            if (!isApply) return;

            _dialog.ShowDialog(nameof(MeroShareApplicationDialogPage), new DialogParameters
            {
                { "ShareInfo", item}
            }, result =>
            {
                if (result?.Result == ButtonResult.OK)
                {
                    RefreshCommand.Execute();
                }
            });
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
         continuationCallback(true);   
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
           return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
