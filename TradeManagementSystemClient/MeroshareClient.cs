using NepseClient.Commons;
using NepseClient.Commons.Contracts;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;

namespace TradeManagementSystemClient
{
    public class MeroshareClient
    {
        private readonly string _sessionFilePath;
        private string _authorization;
        public IRestClient Client { get; set; }
        public bool IsAuthenticated { get; set; }

        public MeroshareClient()
        {
            _sessionFilePath = Path.Combine(Constants.AppDataPath.Value, "meroshare.session");
            Client = CreateClient();
        }

        public void SaveSession()
        {
            if (!string.IsNullOrEmpty(_authorization))
            {
                File.WriteAllText(_sessionFilePath, _authorization);
            }
        }

        public void RestoreSession()
        {
            if (!File.Exists(_sessionFilePath)) return;

            _authorization = File.ReadAllText(_sessionFilePath);
            if (string.IsNullOrEmpty(_authorization)) return;


            Client.Authenticator = new MeroshareAuthenticator(_authorization);
            IsAuthenticated = true;
        }

        private RestClient CreateClient()
        {
            var client = new RestClient("https://backend.cdsc.com.np");
            //client.ThrowOnDeserializationError = true;
            client.UseNewtonsoftJson();

            return client;
        }

        /// <summary>
        /// Get Depository Participants (Capitals)
        /// </summary>
        public Task<IEnumerable<IMeroshareCapital>> GetCapitalsAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/capital/");
            return Client.ExecuteGetAsync<MeroshareCapitalResponse[]>(request, ct)
                .ContinueWith<IEnumerable<IMeroshareCapital>>(task => task.Result.Data);
        }

        public Task AuthenticateAsync(int clientId, string username, string password, CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(new MeroshareAuthRequest(clientId, username, password));

            return Client.ExecutePostAsync(request, ct)
                .ContinueWith(task =>
                {
                    var response = task.Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        IsAuthenticated = false;
                        throw new AuthenticationException(response.Content);
                    }
                    else
                    {
                        var authHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("Authorization"));
                        if (authHeader == null)
                        {
                            IsAuthenticated = false;
                            throw new AuthenticationException(response.Content);
                        }
                        else
                        {
                            _authorization = authHeader.Value?.ToString();
                            var authenticator = new MeroshareAuthenticator(_authorization);
                            Client.Authenticator = authenticator;
                            IsAuthenticated = true;
                        }
                    }
                });
        }

        public Task LogoutAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/auth/logout/");
            return Client.ExecuteGetAsync(request, ct)
                .ContinueWith(task =>
                {
                    IsAuthenticated = false;
                    Client.Authenticator = null;
                });
        }

        public Task<IMeroshareOwnDetail> GetOwnDetailsAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/meroShare/ownDetail/");
            return Client.ExecuteGetAsync<MeroshareOwnDetailResponse>(request, ct)
                .ContinueWith<IMeroshareOwnDetail>(EnsureAuthenticated, ct);
        }

        public Task<IEnumerable<string>> GetMySharesAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/api/myPurchase/myShare/");
            return Client.ExecuteGetAsync<string[]>(request, ct)
                .ContinueWith<IEnumerable<string>>(EnsureAuthenticated, ct);
        }

        public Task<IMeroshareViewMyPurchase> ViewMyPurchaseAsync(string demat, string scrip, CancellationToken ct = default)
        {
            var request = new RestRequest("/api/myPurchase/view/");
            request.AddJsonBody(new MeroshareViewMyPurchaseRequest { Demat = demat, Scrip = scrip });
            return Client.ExecutePostAsync<MeroshareViewMyPurchaseResponse>(request, ct)
                .ContinueWith<IMeroshareViewMyPurchase>(EnsureAuthenticated, ct);
        }

        public Task<IEnumerable<IMeroshareSearchMyPurchaseRespose>> SearchMyPurchaseAsync(string demat, string scrip, CancellationToken ct = default)
        {
            var request = new RestRequest("/api/myPurchase/search/");
            request.AddJsonBody(new MeroshareViewMyPurchaseRequest { Demat = demat, Scrip = scrip });
            return Client.ExecutePostAsync<MeroshareSearchMyPurchaseRespose[]>(request, ct)
                .ContinueWith<IEnumerable<IMeroshareSearchMyPurchaseRespose>>(EnsureAuthenticated, ct);
        }

        public T EnsureAuthenticated<T>(Task<IRestResponse<T>> task)
        {
            var response = task.Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                IsAuthenticated = false;
                throw new AuthenticationException(response.Content);
            }

            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content);
            }

            return response.Data;
        }
    }
}
