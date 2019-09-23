namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    public class PlacingOrderWithNoIntentToExecuteRuleRuleBreach : IPlacingOrdersWithNoIntentToExecuteRuleBreach
    {
        public PlacingOrderWithNoIntentToExecuteRuleRuleBreach(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            IFactorValue factorValue,
            decimal meanPrice,
            decimal sdPrice,
            IReadOnlyCollection<ProbabilityOfExecution> probabilityForOrders,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameters,
            ISystemProcessOperationRunRuleContext ctx,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.Window = window;
            this.Trades = trades;
            this.Security = security;

            this.FactorValue = factorValue;
            this.Parameters = parameters;

            this.MeanPrice = meanPrice;
            this.StandardDeviationPrice = sdPrice;
            this.ProbabilityForOrders = probabilityForOrders ?? new List<ProbabilityOfExecution>();

            this.RuleParameterId = parameters.Id;
            this.SystemOperationId = ctx.Id();
            this.CorrelationId = ctx.CorrelationId();
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

        public decimal MeanPrice { get; set; }

        public IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters Parameters { get; set; }

        public IReadOnlyCollection<ProbabilityOfExecution> ProbabilityForOrders { get; set; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public decimal StandardDeviationPrice { get; set; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public class ProbabilityOfExecution
        {
            public ProbabilityOfExecution(
                string orderId,
                decimal sd,
                decimal mean,
                decimal observed,
                decimal probabilityNormal,
                decimal sigma)
            {
                this.OrderId = orderId;
                this.Sd = sd;
                this.Mean = mean;
                this.Observed = observed;
                this.ProbabilityNormal = probabilityNormal;
                this.Sigma = sigma;
            }

            public decimal Mean { get; set; }

            public decimal Observed { get; set; }

            public string OrderId { get; set; }

            public decimal ProbabilityNormal { get; set; }

            public decimal Sd { get; set; }

            public decimal Sigma { get; set; }

            public override string ToString()
            {
                return
                    $"Order: {this.OrderId} had a probability of execution of :{this.ProbabilityNormal * 100}% Sd: {this.Sd} Mean: {this.Mean} Value(Z): {this.Observed} Sigma: {this.Sigma}";
            }
        }
    }
}