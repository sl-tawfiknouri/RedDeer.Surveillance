﻿using System;
using DomainV2.Financial;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Layering
{
    public class LayeringRuleBreach : ILayeringRuleBreach
    {
        public LayeringRuleBreach(
            ILayeringRuleParameters parameters,
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            RuleBreachDescription bidirectionalTradeBreach, 
            RuleBreachDescription dailyVolumeTradeBreach,
            RuleBreachDescription windowVolumeTradeBreach,
            RuleBreachDescription priceMovementBreach)
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
        public FinancialInstrument Security { get; }

        public RuleBreachDescription BidirectionalTradeBreach { get; }
        public RuleBreachDescription DailyVolumeTradeBreach { get; }
        public RuleBreachDescription WindowVolumeTradeBreach { get; }
        public RuleBreachDescription PriceMovementBreach { get; }

        public bool IsBackTestRun { get; set; }
    }
}
