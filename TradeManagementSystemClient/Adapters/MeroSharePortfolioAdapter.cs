
using System.Collections;
using System.Collections.Generic;

using NepseClient.Commons.Contracts;

using TradeManagementSystemClient.Models.Responses;

namespace TradeManagementSystemClient.Adapters
{
    public class MeroSharePortfolioAdapter : IScripResponse
    {
        private readonly MeroShareMyPortfolio _portfolio;

        public MeroSharePortfolioAdapter(MeroShareMyPortfolio portfolio, IDictionary<string, MeroshareViewMyPurchaseResponse> dict)
        {
            _portfolio = portfolio;
            if (dict.TryGetValue(_portfolio.Script, out var value))
            {
                WaccValue = (float)value.AverageBuyRate;
                DailyGain = LTPTotal - PreviousTotal;
                TotalGain = LTPTotal - (float)value.TotalCost;
                TotalCost = (float)value.TotalCost;
            }
        }

        public string Scrip => _portfolio.Script;
        public string Name => _portfolio.ScriptDesc;
        public float FreeBalance => 0;
        public float TotalBalance => 0;
        public float CurrentBalance => (float)_portfolio.CurrentBalance;
        public float LastTransactionPrice => (float)_portfolio.LastTransactionPrice;
        public float PreviousClosePrice => (float)_portfolio.PreviousClosingPrice;
        public float LTPTotal => (float)_portfolio.ValueOfLastTransPrice;
        public float PreviousTotal => (float)_portfolio.ValueOfPrevClosingPrice;
        public float WaccValue { get; }
        public float DailyGain { get; }
        public float TotalGain { get; }
        public float TotalCost { get; }
    }
}
