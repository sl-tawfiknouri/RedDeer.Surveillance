using System;
using Domain.Equity;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Layering
{
    public class LayeringRuleBreach : ILayeringRuleBreach
    {
        public LayeringRuleBreach(
            ILayeringRuleParameters parameters,
            TimeSpan window,
            ITradePosition trades,
            Security security,
            bool bidirectionalTradeBreach, 
            bool dailyVolumeTradeBreach,
            bool windowVolumeTradeBreach,
            bool priceMovementBreach)
        {
            Parameters = parameters;
            Window = window;
            Trades = trades;
            Security = security;
            BidirectionalTradeBreach = bidirectionalTradeBreach;
            DailyVolumeTradeBreach = dailyVolumeTradeBreach;
            WindowVolumeTradeBreach = windowVolumeTradeBreach;
            PriceMovementBreach = priceMovementBreach;
        }

        public ILayeringRuleParameters Parameters { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public Security Security { get; }

        public bool BidirectionalTradeBreach { get; }
        public bool DailyVolumeTradeBreach { get; }
        public bool WindowVolumeTradeBreach { get; }
        public bool PriceMovementBreach { get; }
    }
}
