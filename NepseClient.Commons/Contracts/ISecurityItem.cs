using System;

namespace NepseClient.Commons.Contracts
{
    public interface ISecurityItem
    {
        float Change { get; set; }
        float High { get; set; }
        DateTime LastTradedDateTime { get; }
        float LastTradedPrice { get; set; }
        int LastTradedVolume { get; set; }
        float Low { get; set; }
        float Open { get; set; }
        float PercentChange { get; set; }
        float PreviousClose { get; set; }
        string Symbol { get; set; }
        int Volume { get; set; }
        int PolarChange { get; }
    }
}