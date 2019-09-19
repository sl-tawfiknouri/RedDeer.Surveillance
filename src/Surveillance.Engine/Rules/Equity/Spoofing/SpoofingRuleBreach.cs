namespace Surveillance.Engine.Rules.Rules.Equity.Spoofing
{
    using System;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;

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
            ISpoofingRuleEquitiesParameters spoofingEquitiesParameters,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;

            this.Window = window;
            this.Security = security;
            this.MostRecentTrade = mostRecentTrade;

            var totalTrades = fulfilledTradePosition.Get().ToList();
            totalTrades.AddRange(cancelledTradePosition.Get());
            this.Trades = new TradePosition(totalTrades);
            this.TradesInFulfilledPosition = fulfilledTradePosition;
            this.CancelledTrades = cancelledTradePosition;

            this.RuleParameterId = spoofingEquitiesParameters?.Id ?? string.Empty;
            this.SystemOperationId = operationContext.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = spoofingEquitiesParameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public ITradePosition CancelledTrades { get; }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public string Description { get; set; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        /// <summary>
        ///     The trade whose fulfillment triggered the rule breach. This is a constituent of trades but not cancelled trades.
        /// </summary>
        public Order MostRecentTrade { get; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public ITradePosition TradesInFulfilledPosition { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }
    }
}