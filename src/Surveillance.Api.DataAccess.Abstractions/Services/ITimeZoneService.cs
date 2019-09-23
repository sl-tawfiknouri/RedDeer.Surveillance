namespace Surveillance.Api.DataAccess.Abstractions.Services
{
    using System;
    using System.Collections.Generic;

    public interface ITimeZoneService
    {
        /// <summary>
        ///     For a given timezone, split a date range in to segments for each daylight saving clock change.
        /// </summary>
        /// <param name="tzName">Name from TZ database https://en.wikipedia.org/wiki/List_of_tz_database_time_zones</param>
        /// <returns>List of segments including UTC date range and UTC hours offset</returns>
        IEnumerable<ITimeZoneOffsetSegment> GetOffsetSegments(DateTime fromUtc, DateTime toUtc, string tzName);
    }
}