using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;

namespace DomainV2.Financial.Interfaces
{
    public interface IInterDayHistoryStack
    {
        Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}