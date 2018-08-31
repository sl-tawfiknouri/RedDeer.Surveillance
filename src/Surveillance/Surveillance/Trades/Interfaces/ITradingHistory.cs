using System;
using System.Collections.Generic;
using Domain.Equity.Trading.Orders;

namespace Surveillance.Trades.Interfaces
{
    public interface ITradingHistory
    {
        void Add(TradeOrderFrame frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        IList<TradeOrderFrame> ActiveTradeHistory();
    }
}