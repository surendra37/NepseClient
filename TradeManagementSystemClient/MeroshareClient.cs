using NepseClient.Commons;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Requests.MeroShare;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Models.Responses.MeroShare;
using TradeManagementSystemClient.Utils;

namespace TradeManagementSystemClient
{
    public class MeroshareClient : IDisposable
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

        public IRestClient Client { get; set; }
        public Func<MeroshareAuthRequest> PromptCredential { get; set; }
        public MeroshareClient(IConfiguration configuration)
        {
            _configuration = configuration.Meroshare;

            Client = RestClientUtils.CreateNewClient(_configuration.BaseUrl);
            RestoreSession();
        }

        private void RestoreSession()
        {
            if (!string.IsNullOrEmpty(_configuration.AuthHeader))
            {
                Client.Authenticator = new MeroshareAuthenticator(_configuration.AuthHeader);
            }
        }

        #region Authentication
        public virtual void Authorize()
        {
            Log.Debug("Authorizing...");
            var cred = PromptCredential?.Invoke();
            if (cred is null) throw new AuthenticationException("Not Authorized");
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(MeroshareAuthRequest credentials)
        {
            Log.Debug("Signing in...");
            Client = RestClientUtils.CreateNewClient(_configuration.BaseUrl);
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(credentials);

            var response = Client.Post(request);
            if (!response.IsSuccessful)
            {
                Client = null;
                throw new AuthenticationException(response.Content);
            }

            var authHeader = response.Headers
                .FirstOrDefault(x => x.Name.Equals("Authorization"))?.Value.ToString();
            if (string.IsNullOrEmpty(authHeader)) 
                throw new AuthenticationException("Authorization header not found");
            Client.Authenticator = new MeroshareAuthenticator(authHeader);

            // Store session
            _configuration.AuthHeader = authHeader;
            _configuration.Save();

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
            Client = null;
            Log.Debug(response.Content);

            // Remove all stored values
            _me = null;
            _banks = null;
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
            var response = AuthorizedGet<MeroshareOwnDetailResponse>(request);
            return response.Data;
        }

        public string[] GetMyShares()
        {
            Log.Debug("Getting my shares");
            var request = new RestRequest("/api/myPurchase/myShare/");
            var response = AuthorizedGet<string[]>(request);
            return response.Data;
        }

        public MeroshareViewMyPurchaseResponse ViewMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            Log.Debug("View my purchase");
            var request = new RestRequest("/api/myPurchase/view/");
            request.AddJsonBody(body);
            var response = AuthorizedPost<MeroshareViewMyPurchaseResponse>(request);
            return response.Data;
        }

        public MeroshareSearchMyPurchaseRespose[] SearchMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            var request = new RestRequest("/api/myPurchase/search/");
            request.AddJsonBody(body);
            var response = AuthorizedPost<MeroshareSearchMyPurchaseRespose[]>(request);
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

            var response = AuthorizedPost<MerosharePortfolioResponse>(request);
            return response.Data;
        }

        public MeroshareViewMyPurchaseResponse[] ReadWaccFromFile()
        {
            var path = Path.Combine(Constants.AppDataPath.Value, "wacc.json");
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
#else
            SignOut();
#endif
        }

        #region Helper Methods
        private IRestResponse<T> AuthorizedGet<T>(IRestRequest request)
        {
            if (Client is null) Authorize();

            var response = Client.Get<T>(request);
            if (response.IsUnAuthorized())
            {
                Authorize();
                return Client.Get<T>(request);
            }
            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content);
            }
            return response;
        }
        private IRestResponse<T> AuthorizedPost<T>(IRestRequest request)
        {
            if (Client is null) Authorize();

            var response = Client.Post<T>(request);
            if (response.IsUnAuthorized())
            {
                Authorize();
                return Client.Post<T>(request);
            }
            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content);
            }

            return response;
        }
        #endregion

        #region ASBA
        public ApplicationReportResponse GetOldApplicationReport()
        {
            var request = new RestRequest("/api/meroShare/migrated/applicantForm/search");
            var body = new GetApplicationReport("VIEW");
            request.AddJsonBody(body);

            var response = AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }
        public ApplicationReportResponse GetApplicationReport()
        {
            var request = new RestRequest("/api/meroShare/applicantForm/active/search/");
            var body = new GetApplicationReport("VIEW_APPLICANT_FORM_COMPLETE");
            request.AddJsonBody(body);

            var response = AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }

        public OldApplicationReportDetailResponse GetApplicationReportDetails(ApplicationReportItem report)
        {
            // For share information
            var request = new RestRequest($"/api/meroShare/active/{report.CompanyShareId}"); //288  // CompanyShareId
            var response = AuthorizedGet<OldApplicationReportDetailResponse>(request);
            return response.Data;

        }
        public OldApplicationReportDetailResponse GetOldApplicationReportDetails(ApplicationReportItem report)
        {
            // For alloted information
            var request = new RestRequest($"/api/meroShare/migrated/applicantForm/report/{report.ApplicantFormId}"); //8549728 // ApplicantFormId
            var response = AuthorizedGet<OldApplicationReportDetailResponse>(request);
            return response.Data;
        }

        public ApplicationReportResponse GetApplicableIssues()
        {
            var request = new RestRequest("/api/meroShare/companyShare/applicableIssue/");
            var body = new GetApplicationReport("VIEW_APPLICABLE_SHARE");
            request.AddJsonBody(body);
            var response = AuthorizedPost<ApplicationReportResponse>(request);
            return response.Data;
        }

        private MeroshareCapitalResponse[] GetMyBanks()
        {
            var request = new RestRequest("/api/meroShare/bank/");
            var response = AuthorizedGet<MeroshareCapitalResponse[]>(request);
            return response.Data;
        }

        public BankDetailResponse GetBankDetails(string id)
        {
            var request = new RestRequest($"/api/meroShare/bank/{id}"); //44

            var response = AuthorizedGet<BankDetailResponse>(request);
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

            var response = AuthorizedPost<ResponseBase>(request);
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
            var response = AuthorizedPost<ResponseBase>(request);
            return response.Data;
        }
        public ReApplyResponse ReApplyIssue(string companyShareId)
        {
            var request = new RestRequest($"/api/meroShare/applicantForm/reapply/{companyShareId}"); // 305

            var response = AuthorizedGet<ReApplyResponse>(request);
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

            var response = AuthorizedPost<ResponseBase>(request);
            return response.Data;
        }
        #endregion

        public string GetPurchaseDisclaimer()
        {
            var request = new RestRequest("/api/myPurchase/disclaimer/");
            var response = AuthorizedGet<string>(request);
            return response.Data;
        }
    }
}
