using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;
using NepseClient.Commons.Interfaces;
using NepseClient.Commons.Utils;
using NepseClient.Libraries.MeroShare.Constants;
using NepseClient.Libraries.MeroShare.Models.Requests;
using NepseClient.Libraries.MeroShare.Models.Responses;

using Ookii.Dialogs.Wpf;

using RestSharp;

using Serilog;

namespace NepseClient.Libraries.MeroShare
{
    public class MeroshareClient : IAuthorizable, IRestAuthorizableAsync
    {
        private const string _authKey = "meroshare_key";
        private readonly IMeroShareConfiguration _configuration;
        private readonly IStorage _storage;

        public bool IsAuthenticated { get; set; }

        public IRestClient Client { get; }
        public Func<MeroshareAuthRequest> PromptCredential { get; set; }
        public IMemoryCache Cache { get; }

        public MeroshareClient(IConfiguration configuration, IMemoryCache cache, IStorage storage)
        {
            _configuration = configuration.Meroshare;
            _storage = storage;
            Client = RestClientUtils.CreateNewClient("https://backend.cdsc.com.np");
            Cache = cache;

            if (_storage.LocalStorage.TryGetValue(_authKey, out var value))
            {
                IsAuthenticated = true;
                Client.Authenticator = new MeroshareAuthenticator(value);
                Client.CookieContainer = _storage.Container;
            }
        }

        #region Authentication
        public virtual void Authorize()
        {
            SignInAsync().GetAwaiter().GetResult();
        }
        public async Task SignInAsync(CancellationToken ct = default)
        {
            await SignOutAsync(ct);

            var clientId = _configuration.ClientId;
            var dialog = new CredentialDialog
            {
                MainInstruction = $"Please provide mero share credentials",
                Content = "Enter your username and password provided by your broker",
                WindowTitle = "Input MeroShare Credentials",
                Target = "https://backend.cdsc.com.np",
                ShowSaveCheckBox = true,
                ShowUIForSavedCredentials = true,
            };
            using (dialog)
            {
                if (dialog.ShowDialog())
                {
                    var username = dialog.UserName;
                    var password = dialog.Password;
                    await SignInAsync(clientId, username, password, ct); //176//150394//vVy.$3pz7wx#y9S
                    dialog.ConfirmCredentials(true);
                    IsAuthenticated = true;
                }
            }
        }
        private async Task SignInAsync(string clientId, string username, string password, CancellationToken ct)
        {
            Log.Debug("Signing in...");

            var request = new RestRequest("/api/meroShare/auth/");
            var json = new MeroshareAuthRequest(clientId, username, password);
            request.AddJsonBody(json);

            var response = await Client.ExecutePostAsync(request, ct);
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
            _storage.LocalStorage[_authKey] = authHeader;
            _storage.Save();
            Log.Debug("Signed In");
        }
        public async Task SignOutAsync(CancellationToken ct = default)
        {
            Log.Debug("Signing out from MeroShare");
            if (IsAuthenticated)
            {
                var request = new RestRequest("/api/meroShare/auth/logout/");
                request.AddOrUpdateParameter("Accept", "application/json, text/plain, */*", ParameterType.HttpHeader);
                var response = await Client.ExecuteGetAsync(request, ct);
                Log.Debug(response.Content);
                if (response.IsSuccessful)
                {
                    IsAuthenticated = false;
                }
            }
            foreach (var key in CacheKeys.All)
            {
                Cache.Remove(key);
            }
            // Remove all stored values
            Log.Debug("Signed out from MeroShare");
        }
        #endregion

        /// <summary>
        /// Get Depository Participants (Capitals)
        /// </summary>
        public Task<MeroshareCapitalResponse[]> GetCapitalsAsync(CancellationToken ct = default)
        {
            Log.Debug("Getting capitals");
            var request = new RestRequest("/api/meroShare/capital");
            return Client.GetAsync<MeroshareCapitalResponse[]>(request, ct);
        }
        public Task<MeroshareOwnDetailResponse> GetOwnDetailsAsync(bool useCache = true, CancellationToken ct = default)
        {
            if (!useCache) return GetOwnDetailsAsync(ct);

            return Cache.GetOrCreateAsync(CacheKeys.OwnDetail, entry =>
            {
                entry.Size = 1;
                return GetOwnDetailsAsync(ct);
            });
        }
        private Task<MeroshareOwnDetailResponse> GetOwnDetailsAsync(CancellationToken ct = default)
        {
            Log.Debug("Getting own details");
            var request = new RestRequest("/api/meroShare/ownDetail/");
            return this.AuthorizeGetAsync<MeroshareOwnDetailResponse>(request, ct);
        }

        public Task<string[]> GetMySharesAsync(CancellationToken ct = default)
        {
            Log.Debug("Getting my shares");
            var request = new RestRequest("/api/myPurchase/myShare/");
            return this.AuthorizeGetAsync<string[]>(request, ct);
        }

        public Task<MeroshareViewMyPurchaseResponse> ViewMyPurchaseAsync(MeroshareViewMyPurchaseRequest body, CancellationToken ct = default)
        {
            Log.Debug("View my purchase");
            var request = new RestRequest("/api/myPurchase/view/");
            request.AddJsonBody(body);
            return this.AuthorizePostAsync<MeroshareViewMyPurchaseResponse>(request, ct);
        }
        public Task<MeroshareSearchMyPurchaseRespose[]> SearchMyPurchase(MeroshareViewMyPurchaseRequest body, CancellationToken ct = default)
        {
            var request = new RestRequest("/api/myPurchase/search/");
            request.AddJsonBody(body);
            return this.AuthorizePostAsync<MeroshareSearchMyPurchaseRespose[]>(request, ct);
        }
        public async Task<MerosharePortfolioResponse> GetMyPortfoliosAsync(int page = 1, int size = 200, CancellationToken ct = default)
        {
            var me = await GetOwnDetailsAsync(true, ct);
            var body = new GetMyPortfolioRequest
            {
                ClientCode = me.ClientCode,
                Demat = new string[] { me.Demat },
                Page = page,
                Size = size,
                SortAsc = true,
                SortBy = "scrip"
            };
            var request = new RestRequest("/api/meroShareView/myPortfolio");
            request.AddJsonBody(body);

            return await this.AuthorizePostAsync<MerosharePortfolioResponse>(request, ct);
        }

        #region ASBA
        public Task<ApplicationReportResponse> GetOldApplicationReportAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/migrated/applicantForm/search");
            var body = new GetApplicationReport("VIEW");
            request.AddJsonBody(body);

            return this.AuthorizePostAsync<ApplicationReportResponse>(request, ct);
        }
        public Task<ApplicationReportResponse> GetApplicationReportAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/applicantForm/active/search/");
            var body = new GetApplicationReport("VIEW_APPLICANT_FORM_COMPLETE");
            request.AddJsonBody(body);

            return this.AuthorizePostAsync<ApplicationReportResponse>(request, ct);
        }

        public Task<ApplicantFormReportDetail> GetApplicantFormReportDetailAsync(ApplicationReportItem report, CancellationToken ct = default)
        {
            // For alloted information
            var request = new RestRequest($"/api/meroShare/applicantForm/report/detail/{report.ApplicantFormId}"); // 8094132 applicant form id
            return this.AuthorizeGetAsync<ApplicantFormReportDetail>(request, ct);
        }
        public Task<AsbaShareReportDetailResponse> GetAsbaCompanyDetailsAsync(ApplicationReportItem report, CancellationToken ct = default)
        {
            // For share information
            var request = new RestRequest($"/api/meroShare/active/{report.CompanyShareId}"); //288  // CompanyShareId
            return this.AuthorizeGetAsync<AsbaShareReportDetailResponse>(request, ct);

        }
        public Task<ApplicantFormReportDetail> GetOldApplicationReportDetailsAsync(ApplicationReportItem report, CancellationToken ct = default)
        {
            // For alloted information
            var request = new RestRequest($"/api/meroShare/migrated/applicantForm/report/{report.ApplicantFormId}"); //8549728 // ApplicantFormId
            return this.AuthorizeGetAsync<ApplicantFormReportDetail>(request, ct);
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
