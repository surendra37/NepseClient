using NepseClient.Commons.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using System;
using System.IO;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Interfaces;
using TradeManagementSystemClient.Models;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Models.Responses.Tms;
using TradeManagementSystemClient.Utils;

namespace TradeManagementSystemClient
{
    public class TmsClient : IAuthorizable, IDisposable
    {
        private readonly string _cookiPath = "cookies.dat";
        private readonly string _dataPath = "data.dat";

        public AuthenticationDataResponse AuthData { get; private set; }
        private ITmsConfiguration _config;

        public IRestClient Client { get; private set; }
        public Func<AuthenticationRequest> PromptCredentials { get; set; }

        public TmsClient(IConfiguration config)
        {
            _config = config.Tms;
            RestoreSession();
        }
        private void RestoreSession()
        {
            Log.Debug("Restoring session");
            if (File.Exists(_dataPath))
            {
                Client = RestClientUtils.CreateNewClient(_config.BaseUrl);
                Client.CookieContainer = CookieUtils.ReadCookiesFromDisk(_cookiPath);
                var json = File.ReadAllText(_dataPath);
                AuthData = JsonConvert.DeserializeObject<AuthenticationDataResponse>(json);
                Client.Authenticator = new TmsAuthenticator();
            }
        }
        private void SaveSession()
        {
            Log.Debug("Saving session");
            if (Client is not null)
                CookieUtils.WriteCookiesToDisk(_cookiPath, Client.CookieContainer);
            if (AuthData is not null)
            {
                var json = JsonConvert.SerializeObject(AuthData);
                File.WriteAllText(_dataPath, json);
            }
        }
        private void ClearSession()
        {
            Log.Debug("Clearing session");
            Client = null;
            AuthData = null;
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            Log.Debug("Getting business date");
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }
        #endregion

        #region Authentication
        public virtual void Authorize()
        {
            Log.Debug("Authorizing");
            var cred = PromptCredentials?.Invoke();
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(AuthenticationRequest body)
        {
            Log.Debug("Signing in");
            ClearSession();
            Client = RestClientUtils.CreateNewClient(_config.BaseUrl);
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(body);

            var response = Client.Post<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                ClearSession();
                throw new AuthenticationException(response.Content);
            }

            AuthData = response.Data.Data;
            CookieUtils.ParseCookies(response, Client.CookieContainer, Client.BaseUrl);
            Client.Authenticator = new TmsAuthenticator();
            SaveSession();
            Log.Debug("Signed In");
        }
        public void SignOut()
        {
            Log.Debug("Signing out from Tms");
            if (Client is not null)
            {
                Log.Debug("Signing out");
                var request = new RestRequest("/tmsapi/authenticate/logout");
                var response = Client.Post<ResponseBase>(request);
                Log.Information(response.Data.Message);
            }
            ClearSession();
            SaveSession();
            Log.Debug("Signed out from Tms");
        }
        #endregion
        public void Dispose()
        {
            SignOut();
        }

        public WebSocketResponse<WsSecurityResponse> GetSecurities()
        {
            Log.Debug("Getting securities");
            var request = new RestRequest("/tmsapi/ws/top25securities");
            var response = this.AuthorizedGet<WebSocketResponse<WsSecurityResponse>>(request);
            return response.Data;
        }

        public async Task TestWebSockets()
        {
            using (var socket = new ClientWebSocket())
            {
                socket.Options.Cookies = Client.CookieContainer;
                try
                {
                    var url = "wss://tms49.nepsetms.com.np//tmsapi/socketEnd?memberId=149&clientId=1979933&dealerId=1&userId=71620";
                    await socket.ConnectAsync(new Uri(url), CancellationToken.None);

                    var request = "{channel: \"@control\", transaction: \"start_index\"}, payload: {argument: \"undefined\"}}";
                    Log.Debug("Sending data");
                    await Send(socket, request);
                    await Send(socket, "{opCode: \"0xA\"}");
                    Log.Debug("Receiving data");
                    await Receive(socket);
                    Log.Debug("Data received");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to get data");
                }
            }
        }

        static async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var value = await reader.ReadToEndAsync();
                        Log.Debug(value);
                    }
                }
            } while (true);
        }
    }
}
