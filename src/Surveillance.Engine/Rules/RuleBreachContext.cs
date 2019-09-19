namespace Surveillance.Engine.Rules.Rules
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

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
            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.IsBackTestRun = isBackTestRun;
            this.RuleParameterId = ruleParameterId;
            this.SystemOperationId = systemOperationId;
            this.CorrelationId = correlationId;
            this.FactorValue = factorValue;
            this.RuleParameters = ruleParameters;
            this.UniverseDateTime = universeDateTime;
        }

        public string CorrelationId { get; set; }

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