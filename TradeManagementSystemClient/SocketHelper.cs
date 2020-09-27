using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using Serilog;
using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Threading;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;
using WebSocket4Net;

namespace TradeManagementSystem.Nepse
{
    public class SocketHelper
    {
        private bool _isStarted;
        private WebSocket _websocket;
        private readonly INepseClient _client;

        public event EventHandler<SocketResponse[]> DeserializedMessageReceived;

        public SocketHelper(INepseClient client)
        {
            _client = client;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        private void Start()
        {
            if (_isStarted) return;

            try
            {
                Log.Information("Starting socket connection");
                var url = _client.GetSocketUrl();
                Log.Verbose("Opening socket in url: {0}", url);

                _websocket = new WebSocket(url, cookies: _client.GetCookies(), userAgent: "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
                _websocket.EnableAutoSendPing = true;
                _websocket.AutoSendPingInterval = 10;
                _websocket.Opened += Websocket_Opened;
                _websocket.Error += Websocket_Error;
                _websocket.Closed += Websocket_Closed;
                _websocket.MessageReceived += WebSocketMessageReceived;
                _websocket.Open();

                while (_websocket.State != WebSocketState.Open)
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start socket connection");
                throw;
            }
        }

        private void WebSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            var responses = JsonConvert.DeserializeObject<SocketResponse[]>(e.Message);
            DeserializedMessageReceived?.Invoke(sender, responses);
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
            Log.Verbose("Websocket closed");
            _isStarted = false;
        }

        private void Websocket_Error(object sender, ErrorEventArgs e)
        {
            Log.Error(e.Exception, "Web socket error");
        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            Log.Verbose("Websocket opened");
            _isStarted = true;
        }

        public void Stop()
        {
            if (!_isStarted) return;

            _websocket.Close();
            _websocket.Dispose();
        }

        public void SendOpCode()
        {
            Start();
            _websocket.Send("{\"opCode\":\"0xA\"}");
        }
    }
}
