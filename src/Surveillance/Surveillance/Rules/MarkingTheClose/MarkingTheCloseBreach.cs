using System;
using System.Collections.Generic;
using Domain.Trades.Orders;
using DomainV2.Equity;
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
            Security security,
            MarketOpenClose marketClose,
            ITradePosition tradingPosition,
            IMarkingTheCloseParameters parameters,
            VolumeBreach dailyBreach,
            VolumeBreach windowBreach)
        {
            Window = window;
            Security = security ?? throw new ArgumentNullException(nameof(security));

            MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            Trades = tradingPosition ?? new TradePosition(new List<TradeOrderFrame>());
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            DailyBreach = dailyBreach;
            WindowBreach = windowBreach;
        }

        public TimeSpan Window { get; }

        public MarketOpenClose MarketClose { get; }
        public IMarkingTheCloseParameters Parameters { get; }

        public ITradePosition Trades { get; }
        public Security Security { get; }

        public VolumeBreach DailyBreach { get; }
        public VolumeBreach WindowBreach { get; }

    }
}