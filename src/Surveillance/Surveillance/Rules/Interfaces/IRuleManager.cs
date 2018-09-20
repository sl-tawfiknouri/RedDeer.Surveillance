﻿using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;

namespace Surveillance.Rules.Interfaces
{
    public interface IRuleManager
    {
        void RegisterTradingRules(ITradeOrderStream<TradeOrderFrame> stream);
        void RegisterEquityRules(IStockExchangeStream stream);
    }
}