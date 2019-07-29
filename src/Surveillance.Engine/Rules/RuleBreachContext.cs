using System;
using Domain.Core.Financial.Assets;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules
{
    public class RuleBreachContext : IRuleBreachContext
    {
        public RuleBreachContext(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            bool isBackTestRun,
            string ruleParameterId,
            string systemOperationId,
            string correlationId,
            IFactorValue factorValue,
            IRuleParameter ruleParameters,
            DateTime universeDateTime)
        {
            Window = window;
            Trades = trades;
            Security = security;
            IsBackTestRun = isBackTestRun;
            RuleParameterId = ruleParameterId;
            SystemOperationId = systemOperationId;
            CorrelationId = correlationId;
            FactorValue = factorValue;
            RuleParameters = ruleParameters;
            UniverseDateTime = universeDateTime;
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
    }
}
