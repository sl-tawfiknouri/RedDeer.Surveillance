using System;
using System.Collections.Generic;
using DomainV2.Trading;

// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface ITradingHistory
    {
        void Add(Order frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        IList<Order> ActiveTradeHistory();
    }
}