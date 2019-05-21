using Surveillance.Api.DataAccess.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Services
{
    public class TimeZoneOffsetSegment : ITimeZoneOffsetSegment
    {
        public DateTime FromUtc { get; set; }
        public bool FromIncluding { get; set; }
        public DateTime ToUtc { get; set; }
        public bool ToIncluding { get; set; }
        public int HourOffset { get; set; }
    }
}
