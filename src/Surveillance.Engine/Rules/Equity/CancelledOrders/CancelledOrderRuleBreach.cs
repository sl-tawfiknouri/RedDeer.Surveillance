namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    using System;

    using Domain.Core.Financial.Assets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class CancelledOrderRuleBreach : ICancelledOrderRuleBreach
    {
        public CancelledOrderRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext ctx,
            string correlationId,
            ICancelledOrderRuleEquitiesParameters parameters,
            ITradePosition trades,
            FinancialInstrument security,
            bool exceededPercentagePositionCancellations,
            decimal? percentagePositionCancelled,
            int? amountOfPositionCancelled,
            int? amountOfPositionInTotal,
            bool exceededPercentageTradeCountCancellations,
            decimal? percentageTradeCountCancelled,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;
            this.Parameters = parameters;
            this.Trades = trades;
            this.Security = security;
            this.ExceededPercentagePositionCancellations = exceededPercentagePositionCancellations;
            this.PercentagePositionCancelled = percentagePositionCancelled;
            this.AmountOfPositionCancelled = amountOfPositionCancelled;
            this.AmountOfPositionInTotal = amountOfPositionInTotal;
            this.ExceededPercentageTradeCountCancellations = exceededPercentageTradeCountCancellations;
            this.PercentageTradeCountCancelled = percentageTradeCountCancelled;
            this.Window = parameters.Windows.BackwardWindowSize;
            this.RuleParameterId = this.Parameters?.Id ?? string.Empty;
            this.SystemOperationId = ctx.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = this.Parameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public int? AmountOfPositionCancelled { get; }

        public int? AmountOfPositionInTotal { get; }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public string Description { get; set; }

        public bool ExceededPercentagePositionCancellations { get; }

        public bool ExceededPercentageTradeCountCancellations { get; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public ICancelledOrderRuleEquitiesParameters Parameters { get; }

        public decimal? PercentagePositionCancelled { get; }

        public decimal? PercentageTradeCountCancelled { get; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public bool HasBreachedRule()
        {
            return this.ExceededPercentageTradeCountCancellations || this.ExceededPercentagePositionCancellations;
        }
    }
}