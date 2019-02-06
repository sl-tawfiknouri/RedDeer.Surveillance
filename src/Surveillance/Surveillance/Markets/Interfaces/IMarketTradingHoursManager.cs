﻿using System;
using System.Collections.Generic;
using DomainV2.Financial;

namespace Surveillance.Markets.Interfaces
{
    public interface IMarketTradingHoursManager
    {
        ITradingHours GetTradingHoursForMic(string marketIdentifierCode);
        IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(DateTime fromUtc, DateTime toUtc, string marketIdentifierCode);
    }
}