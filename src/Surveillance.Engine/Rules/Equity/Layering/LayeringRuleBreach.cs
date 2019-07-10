using System;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Domain.Core.Financial.Assets;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    public class LayeringRuleBreach : ILayeringRuleBreach
    {
        public LayeringRuleBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            ILayeringRuleEquitiesParameters equitiesParameters,
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            RuleBreachDescription bidirectionalTradeBreach, 
            RuleBreachDescription dailyVolumeTradeBreach,
            RuleBreachDescription windowVolumeTradeBreach,
            RuleBreachDescription priceMovementBreach,
            DateTime universeDateTime)
        {
            FactorValue = factorValue;
            EquitiesParameters = equitiesParameters;
            Window = window;
            Trades = trades;
            Security = security;
            BidirectionalTradeBreach = bidirectionalTradeBreach;
            DailyVolumeTradeBreach = dailyVolumeTradeBreach;
            WindowVolumeTradeBreach = windowVolumeTradeBreach;
            PriceMovementBreach = priceMovementBreach;
            RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
            RuleParameters = equitiesParameters;
            UniverseDateTime = universeDateTime;
        }

        public ILayeringRuleEquitiesParameters EquitiesParameters { get; }
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
        public IFactorValue FactorValue { get; set; }
        public IRuleParameter RuleParameters { get; set; }
        public DateTime UniverseDateTime { get; set; }
    }
}
