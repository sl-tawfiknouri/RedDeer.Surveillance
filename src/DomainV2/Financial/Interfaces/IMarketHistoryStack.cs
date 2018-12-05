using System;
using System.Collections.Generic;
using DomainV2.Equity.Frames;

namespace DomainV2.Financial.Interfaces
{
    public interface IMarketHistoryStack
    {
        Stack<ExchangeFrame> ActiveMarketHistory();
        void Add(ExchangeFrame frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);
        Market Exchange();
    }
}