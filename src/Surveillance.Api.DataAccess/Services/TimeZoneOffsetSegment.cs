namespace Surveillance.Api.DataAccess.Services
{
    using System;

    using Surveillance.Api.DataAccess.Abstractions.Services;

    public class TimeZoneOffsetSegment : ITimeZoneOffsetSegment
    {
        public bool FromIncluding { get; set; }

        public DateTime FromUtc { get; set; }

        public int HourOffset { get; set; }

        public bool ToIncluding { get; set; }

        public DateTime ToUtc { get; set; }
    }
}