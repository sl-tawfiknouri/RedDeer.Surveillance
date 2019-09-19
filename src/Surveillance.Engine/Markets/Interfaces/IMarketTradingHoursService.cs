namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Dates;

    /// <summary>
    /// The MarketTradingHoursService interface.
    /// </summary>
    public interface IMarketTradingHoursService
    {
        /// <summary>
        /// The get trading days within range adjusted to time.
        /// </summary>
        /// <param name="fromUtc">
        /// The from universal time zone.
        /// </param>
        /// <param name="toUtc">
        /// The to universal time zone.
        /// </param>
        /// <param name="marketIdentifierCode">
        /// The market identifier code.
        /// </param>
        /// <returns>
        /// The <see cref="DateRange"/>.
        /// </returns>
        IReadOnlyCollection<DateRange> GetTradingDaysWithinRangeAdjustedToTime(
            DateTime fromUtc,
            DateTime toUtc,
            string marketIdentifierCode);

        /// <summary>
        /// The get trading hours for mic.
        /// </summary>
        /// <param name="marketIdentifierCode">
        /// The market identifier code.
        /// </param>
        /// <returns>
        /// The <see cref="ITradingHours"/>.
        /// </returns>
        ITradingHours GetTradingHoursForMic(string marketIdentifierCode);
    }
}