using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Trades.Orders;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
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
            ITradePosition trades,
            bool marketClosureVirtualProfitComponent)
        {
            Window = parameters.WindowSize;
            Parameters = parameters;
            AbsoluteProfits = absoluteProfits;
            AbsoluteProfitCurrency = absoluteProfitCurrency;
            RelativeProfits = relativeProfits;
            Security = security;
            HasAbsoluteProfitBreach = hasAbsoluteProfitBreach;
            HasRelativeProfitBreach = hasRelativeProfitBreach;
            Trades = trades ?? new TradePosition(new List<TradeOrderFrame>());
            MarketClosureVirtualProfitComponent = marketClosureVirtualProfitComponent;
        }

        public IHighProfitsRuleParameters Parameters { get; }
        public bool HasAbsoluteProfitBreach { get; }
        public bool HasRelativeProfitBreach { get; }
        public decimal? AbsoluteProfits { get; }
        public string AbsoluteProfitCurrency { get; }
        public decimal? RelativeProfits { get; }
        public bool MarketClosureVirtualProfitComponent { get; }
        public Security Security { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
    }
}
