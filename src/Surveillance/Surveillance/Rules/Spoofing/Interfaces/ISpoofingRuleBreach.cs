﻿using Domain.Trades.Orders;
using Surveillance.Rules.Interfaces;
using Surveillance.Trades.Interfaces;
// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleBreach : IRuleBreach
    {
        ITradePosition TradesInFulfilledPosition { get; }
        ITradePosition CancelledTrades { get; }
        TradeOrderFrame MostRecentTrade { get; }
    }
}