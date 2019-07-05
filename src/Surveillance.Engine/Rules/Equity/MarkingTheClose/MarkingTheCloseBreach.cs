using System;
using System.Collections.Generic;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;
using Domain.Core.Financial.Assets;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    public class MarkingTheCloseBreach : IMarkingTheCloseBreach
    {
        public MarkingTheCloseBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            TimeSpan window,
            FinancialInstrument security,
            MarketOpenClose marketClose,
            ITradePosition tradingPosition,
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            VolumeBreach dailyBreach,
            VolumeBreach windowBreach)
        {
            FactorValue = factorValue;

            Window = window;
            Security = security ?? throw new ArgumentNullException(nameof(security));

            MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            Trades = tradingPosition ?? new TradePosition(new List<Order>());
            EquitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;

            RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
            RuleParameters = equitiesParameters;
        }

        public TimeSpan Window { get; }

        public MarketOpenClose MarketClose { get; }
        public IMarkingTheCloseEquitiesParameters EquitiesParameters { get; }

        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public VolumeBreach DailyBreach { get; }
        public VolumeBreach WindowBreach { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
        public IRuleParameter RuleParameters { get; set; }
    }
}