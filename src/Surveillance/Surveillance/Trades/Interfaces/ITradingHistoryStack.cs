using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;

namespace Surveillance.Trades.Interfaces
{
    public interface ITradingHistoryStack
    {
        Stack<Order> ActiveTradeHistory();
        void Add(Order order, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}