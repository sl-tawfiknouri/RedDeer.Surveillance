namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Dates;

    public interface IMarketTradingHoursService
    {
        IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(
            DateTime fromUtc,
            DateTime toUtc,
            string marketIdentifierCode);

        ITradingHours GetTradingHoursForMic(string marketIdentifierCode);
    }
}