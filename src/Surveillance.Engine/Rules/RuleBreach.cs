namespace Surveillance.Engine.Rules.Rules
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class RuleBreach : IRuleBreach
    {
        public RuleBreach(IRuleBreachContext ruleBreachContext, string description, string caseTitle)
        {
            if (ruleBreachContext == null)
            {
                this.Description = description;
                this.CaseTitle = caseTitle;

                return;
            }

            this.Window = ruleBreachContext.Window;
            this.Trades = ruleBreachContext.Trades;
            this.Security = ruleBreachContext.Security;
            this.IsBackTestRun = ruleBreachContext.IsBackTestRun;
            this.RuleParameterId = ruleBreachContext.RuleParameterId;
            this.SystemOperationId = ruleBreachContext.SystemOperationId;
            this.CorrelationId = ruleBreachContext.CorrelationId;
            this.FactorValue = ruleBreachContext.FactorValue;
            this.RuleParameters = ruleBreachContext.RuleParameters;
            this.UniverseDateTime = ruleBreachContext.UniverseDateTime;
            this.Description = description;
            this.CaseTitle = caseTitle;
        }

        public RuleBreach(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            bool isBackTestRun,
            string ruleParameterId,
            string systemOperationId,
            string correlationId,
            IFactorValue factorValue,
            IRuleParameter ruleParameter,
            DateTime universeDateTime,
            string description,
            string caseTitle)
        {
            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.IsBackTestRun = isBackTestRun;
            this.RuleParameterId = ruleParameterId;
            this.SystemOperationId = systemOperationId;
            this.CorrelationId = correlationId;
            this.FactorValue = factorValue;
            this.RuleParameters = ruleParameter;
            this.UniverseDateTime = universeDateTime;
            this.Description = description;
            this.CaseTitle = caseTitle;
        }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public string Description { get; set; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }
    }
}