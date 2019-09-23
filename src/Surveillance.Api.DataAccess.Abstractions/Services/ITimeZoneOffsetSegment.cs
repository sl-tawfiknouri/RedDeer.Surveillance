namespace Surveillance.Api.DataAccess.Abstractions.Services
{
    using System;

    public interface ITimeZoneOffsetSegment
    {
        /// <summary>
        ///     FromIncluding: If true use greater than or equal to, otherwise just use greater than
        /// </summary>
        bool FromIncluding { get; }

        DateTime FromUtc { get; }

        int HourOffset { get; }

        /// <summary>
        ///     ToIncluding: If true use less than or equal to, otherwise just use less than
        /// </summary>
        bool ToIncluding { get; }

        DateTime ToUtc { get; }
    }
}