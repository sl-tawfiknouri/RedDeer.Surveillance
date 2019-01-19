using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;

namespace DomainV2.Financial.Interfaces
{
    public interface IIntraDayHistoryStack
    {
        Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}