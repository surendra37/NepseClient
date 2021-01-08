using System;
using System.Collections.Generic;
using System.Linq;

using NepseApp.Extensions;
using NepseApp.Models;

using Prism.Commands;
using Prism.Mvvm;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareApplyForIssuePageViewModel : ActiveAwareBindableBase
    {
        private readonly MeroshareClient _client;

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareApplyForIssuePageViewModel(IApplicationCommand appCommand,
            MeroshareClient client) : base(appCommand)
        {
            _client = client;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                AppCommand.ShowMessage("Getting applicable issues");
                IsBusy = true;
                Items = _client.GetApplicableIssues().Object;
                IsBusy = false;
                EnqueMessage("Applicable issues updated successfully");
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

        }
    }
}
