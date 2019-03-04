using System;

namespace Domain.Core.Financial.Interfaces
{
    public interface IIntraDayHistoryStack
    {
        Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory();
        void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}