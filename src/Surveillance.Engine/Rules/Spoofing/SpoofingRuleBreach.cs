using System;
using System.Linq;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Spoofing
{
    public class SpoofingRuleBreach : ISpoofingRuleBreach
    {
        public SpoofingRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            TimeSpan window,
            ITradePosition fulfilledTradePosition,
            ITradePosition cancelledTradePosition,
            FinancialInstrument security,
            Order mostRecentTrade,
            ISpoofingRuleParameters spoofingParameters)
        {
            FactorValue = factorValue;

            Window = window;
            Security = security;
            MostRecentTrade = mostRecentTrade;

            var totalTrades = fulfilledTradePosition.Get().ToList();
            totalTrades.AddRange(cancelledTradePosition.Get());
            Trades = new TradePosition(totalTrades);
            TradesInFulfilledPosition = fulfilledTradePosition;
            CancelledTrades = cancelledTradePosition;

            RuleParameterId = spoofingParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public ITradePosition TradesInFulfilledPosition { get; }
        public ITradePosition CancelledTrades { get; }
        public FinancialInstrument Security { get; }

        /// <summary>
        /// The trade whose fulfillment triggered the rule breach. This is a constituent of trades but not cancelled trades.
        /// </summary>
        public Order MostRecentTrade { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
    }
}
