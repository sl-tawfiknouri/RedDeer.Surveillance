using System;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
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
            decimal totalOrdersTradedInWindow,
            DateTime universeDateTime)
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
            RuleParameters = equitiesParameters;
            UniverseDateTime = universeDateTime;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public IHighVolumeRuleEquitiesParameters EquitiesParameters { get; }

        public BreachDetails DailyBreach { get; }
        public BreachDetails WindowBreach { get; }
        public BreachDetails MarketCapBreach { get; }

        public decimal TotalOrdersTradedInWindow { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
        public IRuleParameter RuleParameters { get; set; }
        public DateTime UniverseDateTime { get; set; }

        public class BreachDetails
        {
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                decimal breachThresholdAmount)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdAmount = breachThresholdAmount;
            }

            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                Money breachThresholdMoney,
                Money breachTradedMoney)
            {
                HasBreach = hasBreach;
                BreachPercentage = breachPercentage;
                BreachThresholdMoney = breachThresholdMoney;
                BreachTradedMoney = breachTradedMoney;
            }

            public bool HasBreach { get; }
            public decimal? BreachPercentage { get; }
            public decimal BreachThresholdAmount { get; }
            public Money BreachThresholdMoney { get; }
            public Money BreachTradedMoney { get; }

            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0);
            }
        }
    }
}