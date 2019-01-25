using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.MarkingTheClose
{
    public class MarkingTheCloseBreach : IMarkingTheCloseBreach
    {
        public MarkingTheCloseBreach(
            TimeSpan window,
            FinancialInstrument security,
            MarketOpenClose marketClose,
            ITradePosition tradingPosition,
            IMarkingTheCloseParameters parameters,
            VolumeBreach dailyBreach,
            VolumeBreach windowBreach)
        {
            Window = window;
            Security = security ?? throw new ArgumentNullException(nameof(security));

            MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            Trades = tradingPosition ?? new TradePosition(new List<Order>());
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;

            RuleParameterId = parameters?.Id ?? string.Empty;
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
    }
}