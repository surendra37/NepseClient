using System;

using NepseClient.Libraries.MeroShare.Models.Responses;
using NepseClient.Modules.MeroShare.Extensions;

using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class MyAsbaApplicationReportDialogViewModel : BindableBase, IDialogAware
    {
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

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Share = parameters.GetShareReport();
            Form = parameters.GetFormDetail();
            Title = $"Application Report of {Share.CompanyName}";
        }
    }
}
