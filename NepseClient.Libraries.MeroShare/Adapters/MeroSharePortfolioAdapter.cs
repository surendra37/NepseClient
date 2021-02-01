using System.Collections.Generic;

using NepseClient.Commons.Contracts;
using NepseClient.Libraries.MeroShare.Models.Responses;

namespace NepseClient.Libraries.MeroShare.Adapters
{
    public class MeroSharePortfolioAdapter : IScripResponse
    {
        private readonly MeroShareMyPortfolio _portfolio;

        public MeroSharePortfolioAdapter(MeroShareMyPortfolio portfolio, IDictionary<string, MeroshareViewMyPurchaseResponse> dict)
        {
            _portfolio = portfolio;
            if (dict.TryGetValue(_portfolio.Script, out var value))
            {
                WaccValue = value.AverageBuyRate;
                DailyGain = LTPTotal - PreviousTotal;
                TotalGain = LTPTotal - value.TotalCost;
                TotalCost = value.TotalCost;
            }
        }

        public string Scrip => _portfolio.Script;
        public string Name => _portfolio.ScriptDesc;
        public double CurrentBalance => _portfolio.CurrentBalance;
        public double LastTransactionPrice => _portfolio.LastTransactionPrice;
        public double PreviousClosePrice => _portfolio.PreviousClosingPrice;
        public double LTPTotal => _portfolio.ValueOfLastTransPrice;
        public double PreviousTotal => _portfolio.ValueOfPrevClosingPrice;
        public double WaccValue { get; }
        public double DailyGain { get; }
        public double TotalGain { get; }
        public double TotalCost { get; }
    }
}
