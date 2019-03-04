using System;
using Domain.Core.Financial;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    public class HighVolumeRuleBreach : IHighVolumeRuleBreach
    {
        public HighVolumeRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            IHighVolumeRuleEquitiesParameters equitiesParameters,
            BreachDetails dailyBreach,
            BreachDetails windowBreach,
            BreachDetails marketCapBreach,
            long totalOrdersTradedInWindow)
        {
            FactorValue = factorValue;

            Window = window;
            Trades = trades;
            Security = security;
            EquitiesParameters = equitiesParameters;

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;
            MarketCapBreach = marketCapBreach;

            TotalOrdersTradedInWindow = totalOrdersTradedInWindow;
            RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public IHighVolumeRuleEquitiesParameters EquitiesParameters { get; }

        public BreachDetails DailyBreach { get; }
        public BreachDetails WindowBreach { get; }
        public BreachDetails MarketCapBreach { get; }

        public long TotalOrdersTradedInWindow { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }

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
                Money breachThresholdAmountCurrency,
                Money breachTradedAmountCurrency)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmountCurrency = breachThresholdAmountCurrency;
                BreachTradedAmountCurrency = breachTradedAmountCurrency;
            }

            public bool HasBreach { get; }
            public decimal? BreachPercentage { get; }
            public long BreachThresholdAmount { get; }
            public Money BreachThresholdAmountCurrency { get; }
            public Money BreachTradedAmountCurrency { get; }

            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0);
            }
        }
    }
}