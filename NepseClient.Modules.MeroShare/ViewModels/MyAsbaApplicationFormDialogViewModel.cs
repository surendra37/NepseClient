using System;
using System.Linq;

using Prism.Mvvm;
using Prism.Services.Dialogs;

using TradeManagementSystemClient;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Models.Responses.MeroShare;

namespace NepseClient.Modules.MeroShare.ViewModels
{
    public class MyAsbaApplicationFormDialogViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;
        private readonly MeroshareClient _client;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        #region Share Info
        private ApplicationReportItem _issue;
        public ApplicationReportItem Issue
        {
            get { return _issue; }
            set
            {
                SetProperty(ref _issue, value);
            }
        }

        public string CompanyName => Issue?.CompanyName;
        public string Description => $"{Issue?.SubGroup} ({Issue?.ShareTypeName})";
        public string Scrip => Issue?.Scrip;
        public string ShareGroupName => Issue?.ShareGroupName;
        #endregion

        #region Application
        private string _boid;
        public string Boid
        {
            get { return _boid; }
            set { SetProperty(ref _boid, value); }
        }

        public MeroshareCapitalResponse[] Banks => _client.Banks;

        private MeroshareCapitalResponse _selectedBank;
        public MeroshareCapitalResponse SelectedBank
        {
            get { return _selectedBank; }
            set
            {
                if (SetProperty(ref _selectedBank, value))
                {
                    if (value is null) return;

                    Branch = value.Name;
                    BankDetails = _client.GetBankDetails(value.Id);
                }
            }
        }

        private BankDetailResponse _bankDetails;
        public BankDetailResponse BankDetails
        {
            get { return _bankDetails; }
            set { SetProperty(ref _bankDetails, value); }
        }

        private string _branch;
        public string Branch
        {
            get { return _branch; }
            set { SetProperty(ref _branch, value); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get { return _accountNumber; }
            set { SetProperty(ref _accountNumber, value); }
        }

        private int _appliedKitta;
        public int AppliedKitta
        {
            get { return _appliedKitta; }
            set
            {
                if (SetProperty(ref _appliedKitta, value))
                {
                    RaisePropertyChanged(nameof(Amount));
                }
            }
        }
        public int Amount => AppliedKitta * 100;

        private string _crnNumber;
        public string CrnNumber
        {
            get { return _crnNumber; }
            set { SetProperty(ref _crnNumber, value); }
        }

        private string _disclaimer;
        public string Disclaimer
        {
            get { return _disclaimer; }
            set { SetProperty(ref _disclaimer, value); }
        }
        #endregion

        public MyAsbaApplicationFormDialogViewModel(MeroshareClient client)
        {
            _client = client;
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            // Reset Old Data
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue("ShareInfo", out _issue))
            {
                RaisePropertyChanged(nameof(Issue));
                RaisePropertyChanged(nameof(Banks));

                Title = $"{Issue.CompanyName} ({Issue.Action})";
                SelectedBank = Banks?.FirstOrDefault();

                Boid = _client.Me.Demat;
                Branch = BankDetails.BranchName;
                AccountNumber = BankDetails.AccountNumber;
                var disclaimer = _client.GetPurchaseDisclaimer();

                Disclaimer = disclaimer?.FieldValue;
            }
            else
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Abort));
                return;
            }
        }
    }
}
