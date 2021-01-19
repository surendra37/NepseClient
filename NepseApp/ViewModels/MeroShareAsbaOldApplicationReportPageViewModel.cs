
using System;

using NepseApp.Extensions;
using NepseApp.Models;
using NepseApp.Views;

using Prism.Commands;
using Prism.Services.Dialogs;

using Serilog;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseApp.ViewModels
{
    public class MeroShareAsbaOldApplicationReportPageViewModel : ActiveAwareBindableBase
    {
        private readonly MeroshareClient _client;
        private readonly IDialogService _dialog;
        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MeroShareAsbaOldApplicationReportPageViewModel(IApplicationCommand appCommand, 
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
                AppCommand.ShowMessage("Getting old application report");
                Items = _client.GetOldApplicationReport().Object;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                IsBusy = false;
                EnqueMessage("Failed to get old application report");
                Log.Debug(ex, "Failed to get old application report");
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
                var applicantFormDetails = _client.GetOldApplicationReportDetails(report);

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
