using System;

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