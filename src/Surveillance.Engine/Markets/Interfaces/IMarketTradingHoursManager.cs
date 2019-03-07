using Domain.Core.Financial;
using System;
using System.Collections.Generic;
using Domain.Core.Dates;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IMarketTradingHoursManager
    {
        ITradingHours GetTradingHoursForMic(string marketIdentifierCode);
        IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(DateTime fromUtc, DateTime toUtc, string marketIdentifierCode);
    }
}