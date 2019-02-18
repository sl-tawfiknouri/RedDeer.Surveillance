using System;
using System.Collections.Generic;
using Domain.Equity.TimeBars;

namespace Domain.Financial.Interfaces
{
    public interface IIntraDayHistoryStack
    {
        Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}