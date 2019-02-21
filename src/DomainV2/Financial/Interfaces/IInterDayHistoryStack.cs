using System;
using System.Collections.Generic;
using Domain.Equity.TimeBars;

namespace Domain.Financial.Interfaces
{
    public interface IInterDayHistoryStack
    {
        Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}