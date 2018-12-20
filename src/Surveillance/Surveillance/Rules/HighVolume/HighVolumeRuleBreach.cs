using System;
using DomainV2.Financial;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighVolume
{
    public class HighVolumeRuleBreach : IHighVolumeRuleBreach
    {
        public HighVolumeRuleBreach(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            IHighVolumeRuleParameters parameters,
            BreachDetails dailyBreach,
            BreachDetails windowBreach,
            BreachDetails marketCapBreach,
            long totalOrdersTradedInWindow)
        {
            Window = window;
            Trades = trades;
            Security = security;
            Parameters = parameters;

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;
            MarketCapBreach = marketCapBreach;

            TotalOrdersTradedInWindow = totalOrdersTradedInWindow;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public IHighVolumeRuleParameters Parameters { get; }

        public BreachDetails DailyBreach { get; }
        public BreachDetails WindowBreach { get; }
        public BreachDetails MarketCapBreach { get; }

        public long TotalOrdersTradedInWindow { get; }

        public bool IsBackTestRun { get; set; }

        public class BreachDetails
        {
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                long breachThresholdAmount)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmount = breachThresholdAmount;
            }

            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                CurrencyAmount breachThresholdAmountCurrency,
                CurrencyAmount breachTradedAmountCurrency)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmountCurrency = breachThresholdAmountCurrency;
                BreachTradedAmountCurrency = breachTradedAmountCurrency;
            }

            public bool HasBreach { get; }
            public decimal? BreachPercentage { get; }
            public long BreachThresholdAmount { get; }
            public CurrencyAmount BreachThresholdAmountCurrency { get; }
            public CurrencyAmount BreachTradedAmountCurrency { get; }

            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0);
            }
        }
    }
}