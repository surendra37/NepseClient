using System;
using System.Threading.Tasks;

using NepseClient.Commons.Contracts;
using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Modules.Commons.Extensions;
using NepseClient.Modules.Commons.Interfaces;
using NepseClient.Modules.Commons.Models;
using NepseClient.Modules.MeroShare.Extensions;
using NepseClient.Modules.MeroShare.Views;

using Prism.Commands;
using Prism.Services.Dialogs;

using Serilog;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class MyAsbaOldApplicationReportPageViewModel : ActiveAwareBindableBase, ITabPage
    {
        private readonly MeroshareClient _client;
        private readonly IDialogService _dialog;

        public string Title { get; } = "Old Application Report";

        private ApplicationReportItem[] _items;
        public ApplicationReportItem[] Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MyAsbaOldApplicationReportPageViewModel(IApplicationCommand appCommand,
            MeroshareClient client, IDialogService dialog) : base(appCommand)
        {
            _client = client;
            _dialog = dialog;
        }

        public override async void ExecuteRefreshCommand()
        {
            try
            {
                IsBusy = true;
                AppCommand.ShowMessage("Getting old application report");
                Items = await Task.Run(() =>_client.GetOldApplicationReport().Object);
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

        async void ExecuteViewReportCommand(ApplicationReportItem report)
        {
            try
            {
                AppCommand.ShowMessage("Getting application report");
                var companyDetails = await Task.Run(() =>_client.GetAsbaCompanyDetails(report));
                var applicantFormDetails = await Task.Run(() =>_client.GetOldApplicationReportDetails(report));

                var dialogParams = new DialogParameters()
                    .AddShareReport(companyDetails)
                    .AddApplicantFormDetail(applicantFormDetails);

                _dialog.ShowDialog(nameof(MyAsbaApplicationReportPage), dialogParams, result =>
                {

                });
            }
            catch (Exception ex)
            {
                LogErrorAndEnqueMessage(ex, "Failed to view report");
            }
            AppCommand.HideMessage();
        }
    }
}
