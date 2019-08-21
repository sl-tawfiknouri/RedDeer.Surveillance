namespace Domain.Core.Markets.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Markets.Collections;

    public interface IInterDayHistoryStack
    {
        Stack<EquityInterDayTimeBarCollection> ActiveMarketHistory();

        void Add(EquityInterDayTimeBarCollection frame, DateTime currentTime);

        void ArchiveExpiredActiveItems(DateTime currentTime);

        Market Exchange();
    }
}