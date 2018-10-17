﻿using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Trades.Orders;
using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.Marking_The_Close
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