using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
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
            ISystemProcessOperationRunRuleContext ctx)
        {
            Window = window;
            Trades = trades;
            Security = security;

            FactorValue = factorValue;
            Parameters = parameters;

            MeanPrice = meanPrice;
            StandardDeviationPrice = sdPrice;
            ProbabilityForOrders = probabilityForOrders ?? new List<ProbabilityOfExecution>();

            RuleParameterId = parameters.Id;
            SystemOperationId = ctx.Id();
            CorrelationId = ctx.CorrelationId();

        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public decimal MeanPrice { get; set; }
        public decimal StandardDeviationPrice { get; set; }
        public IFactorValue FactorValue { get; set; }
        public IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters Parameters { get; set; }
        public IReadOnlyCollection<ProbabilityOfExecution> ProbabilityForOrders { get; set; }

        public bool IsBackTestRun { get; set; }

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
                OrderId = orderId;
                Sd = sd;
                Mean = mean;
                Observed = observed;
                ProbabilityNormal = probabilityNormal;
                Sigma = sigma;
            }

            public string OrderId { get; set; }
            public decimal Sd { get; set; }
            public decimal Mean { get; set; }
            public decimal Observed { get; set; }
            public decimal ProbabilityNormal { get; set; }
            public decimal Sigma { get; set; }

            public override string ToString()
            {
                return $"Order: {OrderId} had a probability of execution of :{ProbabilityNormal * 100}% Sd: {Sd} Mean: {Mean} Value(Z): {Observed} Sigma: {Sigma}";
            }
        }
    }
}
