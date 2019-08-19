namespace Domain.Core.Markets.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Markets.Collections;

    public interface IIntraDayHistoryStack
    {
        Stack<EquityIntraDayTimeBarCollection> ActiveMarketHistory();

        void Add(EquityIntraDayTimeBarCollection frame, DateTime currentTime);

        void ArchiveExpiredActiveItems(DateTime currentTime);

        Market Exchange();
    }
}