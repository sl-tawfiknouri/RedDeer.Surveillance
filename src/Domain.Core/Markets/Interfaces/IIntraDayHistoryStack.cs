using System;
using System.Collections.Generic;
using Domain.Core.Markets.Collections;

namespace Domain.Core.Markets.Interfaces
{
    public interface IIntraDayHistoryStack
    {
        Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}