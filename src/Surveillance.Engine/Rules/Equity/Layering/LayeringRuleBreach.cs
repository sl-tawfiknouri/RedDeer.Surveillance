namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class LayeringRuleBreach : ILayeringRuleBreach
    {
        public LayeringRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            ILayeringRuleEquitiesParameters equitiesParameters,
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            RuleBreachDescription bidirectionalTradeBreach,
            RuleBreachDescription dailyVolumeTradeBreach,
            RuleBreachDescription windowVolumeTradeBreach,
            RuleBreachDescription priceMovementBreach,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;
            this.EquitiesParameters = equitiesParameters;
            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.BidirectionalTradeBreach = bidirectionalTradeBreach;
            this.DailyVolumeTradeBreach = dailyVolumeTradeBreach;
            this.WindowVolumeTradeBreach = windowVolumeTradeBreach;
            this.PriceMovementBreach = priceMovementBreach;
            this.RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            this.SystemOperationId = operationContext.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = equitiesParameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public RuleBreachDescription BidirectionalTradeBreach { get; }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public RuleBreachDescription DailyVolumeTradeBreach { get; }

        public string Description { get; set; }

        public ILayeringRuleEquitiesParameters EquitiesParameters { get; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public RuleBreachDescription PriceMovementBreach { get; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public RuleBreachDescription WindowVolumeTradeBreach { get; }
    }
}