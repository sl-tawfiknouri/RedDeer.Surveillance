using Domain.Core.Markets.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Markets.Interfaces
{
    public interface IFixedIncomeIntraDayHistoryStack
    {
        Stack<FixedIncomeIntraDayTimeBarCollection> ActiveMarketHistory();

        void Add(FixedIncomeIntraDayTimeBarCollection frame, DateTime currentTime);

        void ArchiveExpiredActiveItems(DateTime currentTime);

        Market Exchange();
    }
}
