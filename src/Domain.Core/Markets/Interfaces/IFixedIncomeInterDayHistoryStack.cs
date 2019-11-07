using Domain.Core.Markets.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Core.Markets.Interfaces
{
    public interface IFixedIncomeInterDayHistoryStack
    {
        Stack<FixedIncomeInterDayTimeBarCollection> ActiveMarketHistory();

        void Add(FixedIncomeInterDayTimeBarCollection frame, DateTime currentTime);

        void ArchiveExpiredActiveItems(DateTime currentTime);

        Market Exchange();
    }
}
