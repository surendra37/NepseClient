﻿using NepseClient.Commons;
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

namespace TradeManagementSystemClient
{
    public class MeroshareClient : IDisposable
    {
        private readonly IMeroShareConfiguration _configuration;
        private bool _isAuthenticated;
        private MeroshareOwnDetailResponse _me;
        private readonly object _authLock = new object();
        private MeroshareOwnDetailResponse Me
        {
            get
            {
                lock (_authLock)
                {
                    if (_me is null)
                        _me = GetOwnDetails();
                }
                return _me;
            }
        }
        public IRestClient Client { get; set; }
        public Func<MeroshareAuthRequest> PromptCredential { get; set; }
        public MeroshareClient(IConfiguration configuration)
        {
            Client = new RestClient(configuration.Meroshare.BaseUrl)
            {
                CookieContainer = new System.Net.CookieContainer(),
            };
            Client.UseNewtonsoftJson(new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
            _configuration = configuration.Meroshare;

            if (!string.IsNullOrEmpty(_configuration.AuthHeader))
            {
                Client.Authenticator = new MeroshareAuthenticator(_configuration.AuthHeader);
                _isAuthenticated = true;
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
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(credentials);

            var response = Client.Post(request);
            if (!response.IsSuccessful)
            {
                _isAuthenticated = false;
                throw new AuthenticationException(response.Content);
            }

            var authHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("Authorization"));
            if (authHeader is null) throw new AuthenticationException("Authorization header not found");
            var value = authHeader.Value?.ToString();
            _configuration.AuthHeader = value;
            _configuration.Save();
            Client.Authenticator = new MeroshareAuthenticator(value);
            _isAuthenticated = true;

            Log.Debug("Signed In");
        }
        private void SignOut()
        {
            Log.Debug("Signing out from MeroShare");
            if (!_isAuthenticated)
            {
                Log.Debug("Not authorized. No sign out required");
                return;
            }
            var request = new RestRequest("/api/meroShare/auth/logout/");
            var response = Client.Get(request);
            Client.Authenticator = null;
            Log.Debug(response.Content);
            _isAuthenticated = false;
            _configuration.AuthHeader = string.Empty;
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

        public MeroshareOwnDetailResponse GetOwnDetails()
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
        #endregion
    }
}
