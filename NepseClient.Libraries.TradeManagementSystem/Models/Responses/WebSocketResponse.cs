using System;
namespace NepseClient.Libraries.TradeManagementSystem.Models.Responses
{
    public class WebSocketHeader
    {
        public string Channel { get; set; }
        public string Transaction { get; set; }
        public object TnxCode { get; set; }
    }

    public class WsSecurityResponse
    {
        public string Symbol { get; set; }
        public int Volume { get; set; }
        public double Ltp { get; set; }
        public double PercentChange { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public int LastTradedVolume { get; set; }
        public int[] LastTradedTime { get; set; }

        public DateTime LastTradeTime
        {
            get
            {
                var output = new DateTime(LastTradedTime[0], LastTradedTime[1], LastTradedTime[2],
                    LastTradedTime[3], LastTradedTime[4], LastTradedTime[5]);
                return output;
            }
        }
        public double Change { get; set; }
        public double PreviousClose { get; set; }
    }

    public class Payload<T>
    {
        public T[] Data { get; set; }
    }

    public class WebSocketResponse<T>
    {
        public WebSocketHeader Header { get; set; }
        public Payload<T> Payload { get; set; }
    }
}
