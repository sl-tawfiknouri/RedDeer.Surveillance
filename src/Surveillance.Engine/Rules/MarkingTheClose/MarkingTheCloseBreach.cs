using System;
using System.Collections.Generic;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.MarkingTheClose
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
            IMarkingTheCloseParameters parameters,
            VolumeBreach dailyBreach,
            VolumeBreach windowBreach)
        {
            FactorValue = factorValue;

            Window = window;
            Security = security ?? throw new ArgumentNullException(nameof(security));

            MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            Trades = tradingPosition ?? new TradePosition(new List<Order>());
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;

            RuleParameterId = parameters?.Id ?? string.Empty;
            SystemOperationId = operationContext.Id.ToString();
            CorrelationId = correlationId;
        }

        public TimeSpan Window { get; }

        public MarketOpenClose MarketClose { get; }
        public IMarkingTheCloseParameters Parameters { get; }

        public ITradePosition Trades { get; }
        public FinancialInstrument Security { get; }

        public VolumeBreach DailyBreach { get; }
        public VolumeBreach WindowBreach { get; }

        public bool IsBackTestRun { get; set; }
        public string RuleParameterId { get; set; }
        public string SystemOperationId { get; set; }
        public string CorrelationId { get; set; }
        public IFactorValue FactorValue { get; set; }
    }
}