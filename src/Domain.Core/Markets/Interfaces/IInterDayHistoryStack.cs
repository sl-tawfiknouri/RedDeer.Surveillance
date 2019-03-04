using Domain.Core.Financial.Markets;
using Domain.Equity.TimeBars;
using System;
using System.Collections.Generic;

namespace Domain.Core.Financial.Interfaces
{
    public interface IInterDayHistoryStack
    {
        Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}