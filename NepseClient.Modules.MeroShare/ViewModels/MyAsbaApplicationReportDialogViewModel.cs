using System;

using NepseClient.Libraries.MeroShare;
using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Modules.MeroShare.Extensions;

using Prism.Mvvm;
using Prism.Services.Dialogs;

using Serilog;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class MyAsbaApplicationReportDialogViewModel : BindableBase, IDialogAware
    {
        private readonly MeroshareClient _client;
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        private AsbaShareReportDetailResponse _share;
        public AsbaShareReportDetailResponse Share
        {
            get { return _share; }
            set { SetProperty(ref _share, value); }
        }

        private ApplicantFormReportDetail _form;

        public ApplicantFormReportDetail Form
        {
            get { return _form; }
            set { SetProperty(ref _form, value); }
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            try
            {
                var report = parameters.GetReport();
                var isOldReport = parameters.GetReportType();
                
                Title = $"Application Report of {report.CompanyName}";

                Share = await _client.GetAsbaCompanyDetailsAsync(report);
                if (isOldReport)
                {
                    Form = await _client.GetOldApplicationReportDetailsAsync(report);
                }
                else
                {
                    Form = await _client.GetApplicantFormReportDetailAsync(report);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get report");
            }
        }

        public MyAsbaApplicationReportDialogViewModel(MeroshareClient client)
        {
            _client = client;
        }
    }
}
