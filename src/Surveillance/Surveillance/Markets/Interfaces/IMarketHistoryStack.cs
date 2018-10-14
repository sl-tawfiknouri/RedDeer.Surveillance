﻿using System;
using System.Collections.Generic;
using Domain.Equity.Frames;
using Domain.Market;

namespace Surveillance.Markets.Interfaces
{
    public interface IMarketHistoryStack
    {
        void Add(ExchangeFrame frame, DateTime currentTime);
        void ArchiveExpiredActiveItems(DateTime currentTime);

        /// <summary>
        /// Does not provide access to the underlying collection via reference
        /// Instead it returns a new list with the same underlying elements
        /// </summary>
        Stack<ExchangeFrame> ActiveTradeHistory();

        StockExchange Exchange();
    }
}
