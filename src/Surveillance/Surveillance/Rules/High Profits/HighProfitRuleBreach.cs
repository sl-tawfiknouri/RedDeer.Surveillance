using Domain.Equity;
using Surveillance.Rule_Parameters.Interfaces;
using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.High_Profits
{
    public class HighProfitRuleBreach : IHighProfitRuleBreach
    {
        public HighProfitRuleBreach(
            IHighProfitsRuleParameters parameters,
            decimal? absoluteProfits,
            string absoluteProfitCurrency,
            decimal? relativeProfits,
            Security security,
            bool hasAbsoluteProfitBreach,
            bool hasRelativeProfitBreach,
            ITradePosition trades)
        {
            Parameters = parameters;
            AbsoluteProfits = absoluteProfits;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            RelativeProfits = relativeProfits;
            Security = security;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            Trades = trades ?? new TradePosition(new List<TradeOrderFrame>(), null, null, null);
        }

        public IHighProfitsRuleParameters Parameters { get; }
        public bool HasAbsoluteProfitBreach { get; }
        public bool HasRelativeProfitBreach { get; }
        public decimal? AbsoluteProfits { get; }
        public string AbsoluteProfitCurrency { get; }
        public decimal? RelativeProfits { get; }
        public Security Security { get; }
        public ITradePosition Trades { get; }
    }
}
