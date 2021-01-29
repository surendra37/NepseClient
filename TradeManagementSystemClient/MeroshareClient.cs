using NepseClient.Commons;
using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;

using RestSharp;

using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Interfaces;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Requests.MeroShare;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Models.Responses.MeroShare;
using TradeManagementSystemClient.Utils;

namespace TradeManagementSystemClient
{
    public class MeroshareClient : IAuthorizable, IDisposable
    {
        private readonly IMeroShareConfiguration _configuration;

        private readonly object _meLock = new object();
        private MeroshareOwnDetailResponse _me;
        public MeroshareOwnDetailResponse Me
        {
            get
            {
                lock (_meLock)
                {
                    if (_me is null)
                        _me = GetOwnDetails();
                }
                return _me;
            }
        }

        private readonly object _bankLock = new object();
        private MeroshareCapitalResponse[] _banks;
        public MeroshareCapitalResponse[] Banks
        {
            get
            {
                lock (_bankLock)
                {
                    if (_banks is null)
                        _banks = GetMyBanks();
                }
                return _banks;
            }
        }

        public bool IsAuthenticated { get; set; }

        public IRestClient Client { get; }
        public Func<MeroshareAuthRequest> PromptCredential { get; set; }
        public MeroshareClient(IConfiguration configuration)
        {
            _configuration = configuration.Meroshare;
            Client = RestClientUtils.CreateNewClient("https://backend.cdsc.com.np");
            RestoreSession();
        }

        private void RestoreSession()
        {
            if (!string.IsNullOrEmpty(_configuration.AuthHeader))
            {
                IsAuthenticated = true;
                Client.Authenticator = new MeroshareAuthenticator(_configuration.AuthHeader);
            }
        }

        private void ClearSession()
        {
            IsAuthenticated = false;
            Client.CookieContainer = new System.Net.CookieContainer();
            Client.Authenticator = null;
            _banks = null;
            _me = null;
        }

        #region Authentication
        public virtual void Authorize()
        {
            Log.Debug("Authorizing...");
            var cred = PromptCredential?.Invoke();
            if (cred is null) throw new AuthenticationException("Authentication cancelled");
            if(string.IsNullOrEmpty(cred.ClientId)) throw new AuthenticationException("Client id is empty. Please select your DP from settings page.");
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(MeroshareAuthRequest credentials)
        {
            Log.Debug("Signing in...");
            ClearSession();
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(credentials);

            var response = Client.Post(request);
            if (!response.IsSuccessful)
            {
                Log.Warning(response.Content);
                throw new AuthenticationException("Not Authorized");
            }

            var authHeader = response.Headers
                .FirstOrDefault(x => x.Name.Equals("Authorization"))?.Value.ToString();
            if (string.IsNullOrEmpty(authHeader))
                throw new AuthenticationException("Authorization header not found");
            Client.Authenticator = new MeroshareAuthenticator(authHeader);

            // Store session
            _configuration.AuthHeader = authHeader;
            _configuration.Save();

            IsAuthenticated = true;
            Log.Debug("Signed In");
        }
        private void SignOut()
        {
            Log.Debug("Signing out from MeroShare");
            if (Client is null)
            {
                Log.Debug("Not authorized. No sign out required");
                return;
            }
            var request = new RestRequest("/api/meroShare/auth/logout/");
            var response = Client.Get(request);
            Log.Debug(response.Content);

            // Remove all stored values
            ClearSession();
            _configuration.AuthHeader = string.Empty;
            _configuration.Save();
            Log.Debug("Signed out from MeroShare");
        }
        #endregion

        /// <summary>
        /// Get Depository Participants (Capitals)
        /// </summary>
        public MeroshareCapitalResponse[] GetCapitals()
        {
            Log.Debug("Getting capitals");
            var request = new RestRequest("/api/meroShare/capital");
            var response = Client.Get<MeroshareCapitalResponse[]>(request);
            return response.Data;
        }

        private MeroshareOwnDetailResponse GetOwnDetails()
        {
            Log.Debug("Getting own details");
            var request = new RestRequest("/api/meroShare/ownDetail/");
            var response = this.AuthorizedGet<MeroshareOwnDetailResponse>(request);
            return response.Data;
        }

        public string[] GetMyShares()
        {
            Log.Debug("Getting my shares");
            var request = new RestRequest("/api/myPurchase/myShare/");
            var response = this.AuthorizedGet<string[]>(request);
            return response.Data;
        }

        public MeroshareViewMyPurchaseResponse ViewMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            Log.Debug("View my purchase");
            var request = new RestRequest("/api/myPurchase/view/");
            request.AddJsonBody(body);
            var response = this.AuthorizedPost<MeroshareViewMyPurchaseResponse>(request);
            return response.Data;
        }

        public MeroshareSearchMyPurchaseRespose[] SearchMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            var request = new RestRequest("/api/myPurchase/search/");
            request.AddJsonBody(body);
            var response = this.AuthorizedPost<MeroshareSearchMyPurchaseRespose[]>(request);
            return response.Data;
        }

        public IEnumerable<MeroshareViewMyPurchaseResponse> GetWaccs(IEnumerable<string> scrips)
        {
            Log.Debug("Getting waccs");
            var me = GetOwnDetails();
            var request = new MeroshareViewMyPurchaseRequest
            {
                Demat = me.Demat,
            };
            foreach (var scrip in scrips)
            {
                request.Scrip = scrip;
                var view = ViewMyPurchase(request);
                var searches = SearchMyPurchase(request);
                if (searches is null || searches.Length == 0)
                {
                    yield return view;
                }
                else
                {
                    var first = searches.First();
                    var myView = new MeroshareViewMyPurchaseResponse
                    {
                        Isin = first.Isin,
                        ScripName = first.Scrip,
                        TotalCost = searches.Sum(x => x.UserPrice * x.TransactionQuantity),
                        TotalQuantity = searches.Sum(x => x.TransactionQuantity),
                        //AverageBuyRate
                    };
                    myView.TotalCost += view.TotalCost;
                    myView.TotalQuantity += view.TotalQuantity;
                    myView.AverageBuyRate = myView.TotalCost / myView.TotalQuantity;

                    yield return myView;
                }

            }
        }
        public MerosharePortfolioResponse GetMyPortfolios(int page = 1)
        {
            var body = new GetMyPortfolioRequest
            {
                ClientCode = Me.ClientCode,
                Demat = new string[] { Me.Demat },
                Page = page,
                Size = 200,
                SortAsc = true,
                SortBy = "scrip"
            };
            var request = new RestRequest("/api/meroShareView/myPortfolio");
            request.AddJsonBody(body);

            var response = this.AuthorizedPost<MerosharePortfolioResponse>(request);
            return response.Data;
        }

        public MeroshareViewMyPurchaseResponse[] ReadWaccFromFile()
        {
            var path = Path.Combine(PathConstants.AppDataPath.Value, "wacc.json");
            if (!File.Exists(path))
            {
                var output = GetWaccs(GetMyShares()).ToArray();
                File.WriteAllText(path, JsonConvert.SerializeObject(output));
                return output;
            }
            else
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<MeroshareViewMyPurchaseResponse[]>(json);
            }
        }

        public void Dispose()
        {
#if DEBUG
            //SignOut();
#else
            SignOut();
#endif
        }

        #region ASBA
        public ApplicationReportResponse GetOldApplicationReport()
        {
            var request = new RestRequest("/api/meroShare/migrated/applicantForm/search");
            var body = new GetApplicationReport("VIEW");
            request.AddJsonBody(body);

            var response = this.AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }
        public ApplicationReportResponse GetApplicationReport()
        {
            var request = new RestRequest("/api/meroShare/applicantForm/active/search/");
            var body = new GetApplicationReport("VIEW_APPLICANT_FORM_COMPLETE");
            request.AddJsonBody(body);

            var response = this.AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }

        public ApplicantFormReportDetail GetApplicantFormReportDetail(ApplicationReportItem report)
        {
            var request = new RestRequest($"/api/meroShare/applicantForm/report/detail/{report.ApplicantFormId}"); // 8094132 applicant form id
            var response = this.AuthorizedGet<ApplicantFormReportDetail>(request);
            return response.Data;
        }

        public AsbaShareReportDetailResponse GetAsbaCompanyDetails(ApplicationReportItem report)
        {
            // For share information
            var request = new RestRequest($"/api/meroShare/active/{report.CompanyShareId}"); //288  // CompanyShareId
            var response = this.AuthorizedGet<AsbaShareReportDetailResponse>(request);
            return response.Data;

        }
        public ApplicantFormReportDetail GetOldApplicationReportDetails(ApplicationReportItem report)
        {
            // For alloted information
            var request = new RestRequest($"/api/meroShare/migrated/applicantForm/report/{report.ApplicantFormId}"); //8549728 // ApplicantFormId
            var response = this.AuthorizedGet<ApplicantFormReportDetail>(request);
            return response.Data;
        }

        public ApplicationReportResponse GetApplicableIssues()
        {
            var request = new RestRequest("/api/meroShare/companyShare/applicableIssue/");
            var body = new GetApplicationReport("VIEW_APPLICABLE_SHARE");
            request.AddJsonBody(body);
            var response = this.AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }

        private MeroshareCapitalResponse[] GetMyBanks()
        {
            var request = new RestRequest("/api/meroShare/bank/");
            var response = this.AuthorizedGet<MeroshareCapitalResponse[]>(request);
            return response.Data;
        }

        public BankDetailResponse GetBankDetails(string id)
        {
            var request = new RestRequest($"/api/meroShare/bank/{id}"); //44

            var response = this.AuthorizedGet<BankDetailResponse>(request);
            return response.Data;
        }

        public ResponseBase ApplyIssue(ApplyIssueRequest body)
        {
            var request = new RestRequest("/api/meroShare/applicantForm/");
            //var bankDetails = GetBankDetails(bankId);
            //var body = new ApplyIssueRequest
            //{
            //    Demat = Me.Demat,
            //    Boid = Me.Boid,
            //    AppliedKitta = appliedKitta,
            //    CrnNumber = crnNumber,
            //    CompanyShareId = companyShareId,
            //    AccountBranchId = bankDetails.AccountBranchId,
            //    AccountNumber = bankDetails.AccountNumber,
            //    BankId = bankDetails.BankId,
            //    CustomerId = bankDetails.Id,
            //};
            request.AddJsonBody(body);

            var response = this.AuthorizedPost<ResponseBase>(request);
            return response.Data;
        }
        public ResponseBase ValidateIssue(int formId, string companyShareId, string transactionPin)
        {
            var request = new RestRequest("/api/meroShare/applicantForm/validate/");
            var body = new ApplicateValidationRequest
            {
                ApplicantFormId = formId,
                CompanyShareId = companyShareId,
                TransactionPIN = transactionPin,
            };
            request.AddJsonBody(body);
            var response = this.AuthorizedPost<ResponseBase>(request);
            return response.Data;
        }
        public ReApplyResponse ReApplyIssue(string companyShareId)
        {
            var request = new RestRequest($"/api/meroShare/applicantForm/reapply/{companyShareId}"); // 305

            var response = this.AuthorizedGet<ReApplyResponse>(request);
            return response.Data;
        }
        public ResponseBase EditIssue(int formId, ApplyIssueRequest body)
        {
            var request = new RestRequest($"/api/meroShare/applicantForm/edit/{formId}");//8094132
            //var bankDetails = GetBankDetails(bankId);
            //var body = new ApplyIssueRequest
            //{
            //    Demat = Me.Demat,
            //    Boid = Me.Boid,
            //    AppliedKitta = appliedKitta,
            //    CrnNumber = crnNumber,
            //    CompanyShareId = companyShareId,
            //    AccountBranchId = bankDetails.AccountBranchId,
            //    AccountNumber = bankDetails.AccountNumber,
            //    BankId = bankDetails.BankId,
            //    CustomerId = bankDetails.Id,
            //};
            request.AddJsonBody(body);

            var response = this.AuthorizedPost<ResponseBase>(request);
            return response.Data;
        }
        #endregion

        public MeroShareDisclaimerResponse GetPurchaseDisclaimer()
        {
            // /api/meroShare/disclaimer/
            var request = new RestRequest("/api/meroShare/disclaimer/");
            var response = this.AuthorizedGet<MeroShareDisclaimerResponse>(request);
            return response.Data;
        }
    }
}
