using System;
using Domain.Core.Financial.Assets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
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
            FactorValue = factorValue;
            Parameters = parameters;
            Trades = trades;
            Security = security;
            ExceededPercentagePositionCancellations = exceededPercentagePositionCancellations;
            PercentagePositionCancelled = percentagePositionCancelled;
            AmountOfPositionCancelled = amountOfPositionCancelled;
            AmountOfPositionInTotal = amountOfPositionInTotal;
            ExceededPercentageTradeCountCancellations = exceededPercentageTradeCountCancellations;
            PercentageTradeCountCancelled = percentageTradeCountCancelled;
            Window = parameters.Windows.BackwardWindowSize;
            RuleParameterId = Parameters?.Id ?? string.Empty;
            SystemOperationId = ctx.Id.ToString();
            CorrelationId = correlationId;
            RuleParameters = Parameters;
            Description = description ?? string.Empty;
            CaseTitle = caseTitle ?? string.Empty;
            UniverseDateTime = universeDateTime;
        }

        public bool HasBreachedRule()
        {
            return ExceededPercentageTradeCountCancellations
                || ExceededPercentagePositionCancellations;
        }

        public ICancelledOrderRuleEquitiesParameters Parameters { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }
        public bool ExceededPercentagePositionCancellations { get; }
        public decimal? PercentagePositionCancelled { get; }
        public int? AmountOfPositionCancelled { get; }
        public int? AmountOfPositionInTotal { get; }
        public bool ExceededPercentageTradeCountCancellations { get; }
        public decimal? PercentageTradeCountCancelled { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
        public IRuleParameter RuleParameters { get; set; }
        public DateTime UniverseDateTime { get; set; }
        public string Description { get; set; }
        public string CaseTitle { get; set; }
    }
}
