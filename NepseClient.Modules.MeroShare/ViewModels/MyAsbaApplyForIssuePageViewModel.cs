using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class MyAsbaApplyForIssuePageViewModel : ActiveAwareBindableBase, ITabPage
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

        public MyAsbaApplyForIssuePageViewModel(IApplicationCommand appCommand,
            MeroshareClient client, IDialogService dialog) : base(appCommand)
        {
            _client = client;
            _dialog = dialog;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                AppCommand.ShowMessage("Getting applicable issues");
                IsBusy = true;
                Items = await Task.Run(() => _client.GetApplicableIssues().Object);
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

            _dialog.ShowDialog(nameof(MyAsbaApplicationFormDialog), new DialogParameters
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
    }
}
