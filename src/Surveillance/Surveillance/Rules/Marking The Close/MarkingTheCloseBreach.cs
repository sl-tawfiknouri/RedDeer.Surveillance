using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Trades.Orders;
using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Marking_The_Close.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

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
            bool hasSellDailyVolumeBreach,
            decimal? sellDailyVolumeBreach,
            bool hasBuyDailyVolumeBreach,
            decimal? buyDailyVolumeBreach)
        {
            Window = window;
            Security = security ?? throw new ArgumentNullException(nameof(security));

            MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            Trades = tradingPosition ?? new TradePosition(new List<TradeOrderFrame>());
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            HasSellDailyVolumeBreach = hasSellDailyVolumeBreach;
            SellDailyVolumeBreach = sellDailyVolumeBreach;
            HasBuyDailyVolumeBreach = hasBuyDailyVolumeBreach;
            BuyDailyVolumeBreach = buyDailyVolumeBreach;
        }

        public TimeSpan Window { get; }

        public MarketOpenClose MarketClose { get; }
        public IMarkingTheCloseParameters Parameters { get; }

        public ITradePosition Trades { get; }
        public Security Security { get; }

        public bool HasBuyDailyVolumeBreach { get; }
        public decimal? BuyDailyVolumeBreach { get; }

        public bool HasSellDailyVolumeBreach { get; }
        public decimal? SellDailyVolumeBreach { get; }
    }
}