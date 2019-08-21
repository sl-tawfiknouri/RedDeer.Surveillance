namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;

    using Domain.Core.Financial.Assets;

    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class RampingRuleBreach : IRampingRuleBreach
    {
        public RampingRuleBreach(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            string ruleParameterId,
            string systemOperationId,
            string correlationId,
            IFactorValue factorValue,
            IRampingStrategySummaryPanel summaryPanel,
            IRampingRuleEquitiesParameters parameters,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.RuleParameterId = ruleParameterId ?? string.Empty;
            this.SystemOperationId = systemOperationId ?? string.Empty;
            this.CorrelationId = correlationId ?? string.Empty;
            this.FactorValue = factorValue;
            this.SummaryPanel = summaryPanel;
            this.RuleParameters = parameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public string Description { get; set; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public IRampingStrategySummaryPanel SummaryPanel { get; set; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }
    }
}