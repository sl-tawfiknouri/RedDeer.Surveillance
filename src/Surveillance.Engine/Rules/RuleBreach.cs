using System;
using Domain.Core.Financial.Assets;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules
{
    public class RuleBreach : IRuleBreach
    {
        public RuleBreach(
            IRuleBreachContext ruleBreachContext,
            string description,
            string caseTitle)
        {
            if (ruleBreachContext == null)
            {
                Description = description;
                CaseTitle = caseTitle;

                return;
            }

            Window = ruleBreachContext.Window;
            Trades = ruleBreachContext.Trades;
            Security = ruleBreachContext.Security;
            IsBackTestRun = ruleBreachContext.IsBackTestRun;
            RuleParameterId = ruleBreachContext.RuleParameterId;
            SystemOperationId = ruleBreachContext.SystemOperationId;
            CorrelationId = ruleBreachContext.CorrelationId;
            FactorValue = ruleBreachContext.FactorValue;
            RuleParameters = ruleBreachContext.RuleParameters;
            UniverseDateTime = ruleBreachContext.UniverseDateTime;
            Description = description;
            CaseTitle = caseTitle;
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
            Window = window;
            Trades = trades;
            Security = security;
            IsBackTestRun = isBackTestRun;
            RuleParameterId = ruleParameterId;
            SystemOperationId = systemOperationId;
            CorrelationId = correlationId;
            FactorValue = factorValue;
            RuleParameters = ruleParameter;
            UniverseDateTime = universeDateTime;
            Description = description;
            CaseTitle = caseTitle;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }
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
