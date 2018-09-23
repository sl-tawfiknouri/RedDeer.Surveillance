using System;
using System.Collections.Generic;
using Domain.Trades.Orders;
// ReSharper disable UnusedMember.Global

namespace Surveillance.Trades.Interfaces
{
    public interface ITradingHistory
    {
        void Add(TradeOrderFrame frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        IList<TradeOrderFrame> ActiveTradeHistory();
    }
}