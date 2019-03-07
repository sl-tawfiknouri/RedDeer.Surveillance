using System;
using System.Collections.Generic;
using Domain.Core.Markets;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface ITradingHistoryStack
    {
        Stack<Order> ActiveTradeHistory();
        void Add(Order order, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}