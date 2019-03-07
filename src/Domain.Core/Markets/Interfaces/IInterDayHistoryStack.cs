using System;
using System.Collections.Generic;
using Domain.Core.Markets.Collections;

namespace Domain.Core.Markets.Interfaces
{
    public interface IInterDayHistoryStack
    {
        Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}