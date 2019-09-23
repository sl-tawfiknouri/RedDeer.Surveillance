namespace Surveillance.Engine.Rules.Rules.Equity.HighVolume
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    using Domain.Core.Markets;

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
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;

            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.EquitiesParameters = equitiesParameters;

            this.DailyBreach = dailyBreach;
            this.WindowBreach = windowBreach;
            this.MarketCapBreach = marketCapBreach;

            this.TotalOrdersTradedInWindow = totalOrdersTradedInWindow;
            this.RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            this.SystemOperationId = operationContext.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = equitiesParameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public BreachDetails DailyBreach { get; }

        public string Description { get; set; }

        public IHighVolumeRuleEquitiesParameters EquitiesParameters { get; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public BreachDetails MarketCapBreach { get; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public decimal TotalOrdersTradedInWindow { get; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public BreachDetails WindowBreach { get; }

        public class BreachDetails
        {
            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                decimal breachThresholdAmount,
                Market venue)
            {
                this.HasBreach = hasBreach;
                this.BreachPercentage = breachPercentage;
                this.BreachThresholdAmount = breachThresholdAmount;
                Venue = venue;
            }

            public BreachDetails(
                bool hasBreach,
                decimal? breachPercentage,
                Money breachThresholdMoney,
                Money breachTradedMoney,
                Market venue)
            {
                this.HasBreach = hasBreach;
                this.BreachPercentage = breachPercentage;
                this.BreachThresholdMoney = breachThresholdMoney;
                this.BreachTradedMoney = breachTradedMoney;
                Venue = venue;
            }

            public decimal? BreachPercentage { get; }

            public decimal BreachThresholdAmount { get; }

            public Money BreachThresholdMoney { get; }

            public Money BreachTradedMoney { get; }

            public bool HasBreach { get; }

            public Market Venue { get; }

            public static BreachDetails None()
            {
                return new BreachDetails(false, null, 0, null);
            }
        }
    }
}