using System;
using System.Collections.Generic;
using DomainV2.Equity.TimeBars;

namespace DomainV2.Financial.Interfaces
{
    public interface IMarketHistoryStack
    {
        Stack<MarketTimeBarCollection> ActiveMarketHistory();
        void Add(MarketTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}