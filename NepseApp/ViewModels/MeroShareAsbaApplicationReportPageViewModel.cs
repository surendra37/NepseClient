using System;

using NepseApp.Views;

using NepseClient.Commons.Contracts;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Extensions;

using Prism.Commands;
using Prism.Services.Dialogs;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaApplicationReportPageViewModel : ActiveAwareBindableBase, ITabPage
    {
        private readonly MeroshareClient _client;
        private readonly IDialogService _dialog;

        public string Title { get; } = "Application Report";

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareAsbaApplicationReportPageViewModel(IApplicationCommand appCommand,
            MeroshareClient client, IDialogService dialog) : base(appCommand)
        {
            _client = client;
            _dialog = dialog;
        }

        public override void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting application report");
                Items = _client.GetApplicationReport().Object;
                if (Items is null)
                {
                    _client.IsAuthenticated = false;
                    Items = _client.GetApplicationReport().Object;
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Log.Debug(ex, "Failed to get application report");
                EnqueMessage("Failed to get application report");
            }
            AppCommand.HideMessage();
        }

        private DelegateCommand<ApplicationReportItem> _viewReportCommand;
        public DelegateCommand<ApplicationReportItem> ViewReportCommand =>
            _viewReportCommand ?? (_viewReportCommand = new DelegateCommand<ApplicationReportItem>(ExecuteViewReportCommand));

        void ExecuteViewReportCommand(ApplicationReportItem report)
        {
            try
            {
                var companyDetails = _client.GetAsbaCompanyDetails(report);
                var applicantFormDetails = _client.GetApplicantFormReportDetail(report);

                var dialogParams = new DialogParameters()
                    .AddShareReport(companyDetails)
                    .AddApplicantFormDetail(applicantFormDetails);

                _dialog.ShowDialog(nameof(ViewAsbaReportDialog), dialogParams, result =>
                {

                });
            }
            catch (Exception ex)
            {
                LogErrorAndEnqueMessage(ex, "Failed to view report");
            }
        }
    }
}
