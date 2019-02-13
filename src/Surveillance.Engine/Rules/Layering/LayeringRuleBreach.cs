using System;
using DomainV2.Financial;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Layering.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Layering
{
    public class LayeringRuleBreach : ILayeringRuleBreach
    {
        public LayeringRuleBreach(
            ISystemProcessOperationContext operationContext,
            string correlationId,
            ILayeringRuleParameters parameters,
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            RuleBreachDescription bidirectionalTradeBreach, 
            RuleBreachDescription dailyVolumeTradeBreach,
            RuleBreachDescription windowVolumeTradeBreach,
            RuleBreachDescription priceMovementBreach)
        {
            Parameters = parameters;
            Window = window;
            Trades = trades;
            Security = security;
            BidirectionalTradeBreach = bidirectionalTradeBreach;
            DailyVolumeTradeBreach = dailyVolumeTradeBreach;
            WindowVolumeTradeBreach = windowVolumeTradeBreach;
            PriceMovementBreach = priceMovementBreach;
            RuleParameterId = parameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public ILayeringRuleParameters Parameters { get; }
        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public RuleBreachDescription BidirectionalTradeBreach { get; }
        public RuleBreachDescription DailyVolumeTradeBreach { get; }
        public RuleBreachDescription WindowVolumeTradeBreach { get; }
        public RuleBreachDescription PriceMovementBreach { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
    }
}
