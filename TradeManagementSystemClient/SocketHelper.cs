using Newtonsoft.Json;

using Serilog;

using SuperSocket.ClientEngine;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using TradeManagementSystemClient.Models.Requests;

using WebSocket4Net;

namespace TradeManagementSystemClient
{
    public class SocketHelper : IDisposable
    {
        private bool _isStarted;
        private WebSocket _websocket;
        private readonly TmsClient _client;
        private System.Timers.Timer _timer;

        //public event EventHandler<SocketResponse[]> DeserializedMessageReceived;

        public SocketHelper(TmsClient client)
        {
            _client = client;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        private string GetSocketUrl()
        {
            if (_client.Client is null) _client.Authorize();

            var baseUrl = _client.Client.BaseUrl;
            var memberId = _client.AuthData.ClientDealerMember.Member.Id;
            var clientId = _client.AuthData.ClientDealerMember.Client.Id;
            var dealerId = _client.AuthData.ClientDealerMember.Dealer.Id;
            var userId = _client.AuthData.User.Id;

            var url = $"wss://{baseUrl.Authority}/tmsapi/socketEnd?memberId={memberId}&clientId={clientId}&dealerId={dealerId}&userId={userId}";
            return url;
        }

        private List<KeyValuePair<string, string>> GetCookies()
        {
            var output = new List<KeyValuePair<string, string>>();
            var cookies = _client.Client.CookieContainer.GetCookies(_client.Client.BaseUrl);
            foreach (Cookie cookie in cookies)
            {
                output.Add(new KeyValuePair<string, string>(cookie.Name, cookie.Value));
            }
            return output;
        }

        private void Start()
        {
            if (_isStarted) return;

            try
            {
                Log.Information("Starting socket connection");
                var url = GetSocketUrl();
                Log.Debug("Opening socket in url: {0}", url);

                _websocket = new WebSocket(url, cookies: GetCookies(), userAgent: "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36")
                {
                    EnableAutoSendPing = true,
                    AutoSendPingInterval = 10
                };
                _websocket.MessageReceived += WebSocketMessageReceived;
                _websocket.Opened += Websocket_Opened;
                _websocket.Error += Websocket_Error;
                _websocket.Closed += Websocket_Closed;
                _websocket.Open();

                while (_websocket.State != WebSocketState.Open)
                {
                    Log.Debug("Waiting for connection to be opened");
                    Thread.Sleep(80);
                }
                _timer = new System.Timers.Timer(10);
                _timer.Elapsed += _timer_Elapsed;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start socket connection");
                throw;
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            SendOpCode();
            _timer.Start();
        }

        public bool Received { get; set; }
        private void WebSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            Log.Debug("Message received: {0}", e.Message);
            Received = true;
            //var responses = JsonConvert.DeserializeObject<SocketResponse[]>(e.Message);
            //DeserializedMessageReceived?.Invoke(sender, responses);
        }

        /// <summary>
        /// Send the socket request
        /// </summary>
        /// <param name="transaction">index, top25securities, ticker</param>
        /// <param name="startOrStop">start or stop the transactions</param>
        public void Send(string transaction, bool startOrStop)
        {
            Start();
            Log.Information("Sending command to socket. Transaction: {0}, Start: {1}", transaction, startOrStop);
            var request = new SocketRequest(startOrStop, transaction);
            var json = JsonConvert.SerializeObject(request);
            Log.Verbose(json);
            _websocket.Send(json);
        }

        private void Websocket_Closed(object sender, EventArgs e)
        {
            Log.Debug("Websocket closed");
            _isStarted = false;
        }

        private void Websocket_Error(object sender, ErrorEventArgs e)
        {
            Log.Error(e.Exception, "Web socket error");
        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            Log.Debug("Websocket opened");
            _isStarted = true;
        }

        public void Stop()
        {
            if (!_isStarted) return;

            _timer.Dispose();
            _websocket.Close();
            _websocket.Dispose();
        }

        public void SendOpCode()
        {
            Start();
            _websocket.Send("{\"opCode\":\"0xA\"}");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}