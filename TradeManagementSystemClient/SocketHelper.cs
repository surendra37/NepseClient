using NepseClient.Commons;
using Newtonsoft.Json;
using Serilog;
using SuperSocket.ClientEngine;
using System;
using System.Threading;
using TradeManagementSystemClient.Models.Requests;
using WebSocket4Net;

namespace TradeManagementSystem.Nepse
{
    public class SocketHelper
    {
        private bool _isStarted;
        private WebSocket _websocket;
        private readonly INepseClient _client;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public SocketHelper(INepseClient client)
        {
            _client = client;
        }

        private void Start()
        {
            if (_isStarted) return;

            try
            {
                Log.Information("Starting socket connection");
                var url = _client.GetSocketUrl();
                Log.Verbose("Opening socket in url: {0}", url);

                _websocket = new WebSocket(url, cookies: _client.GetCookies());
                _websocket.Opened += Websocket_Opened;
                _websocket.Error += Websocket_Error;
                _websocket.Closed += Websocket_Closed;
                _websocket.MessageReceived += MessageReceived;
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
            MessageReceived.Invoke(this, new MessageReceivedEventArgs("[]"));
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
