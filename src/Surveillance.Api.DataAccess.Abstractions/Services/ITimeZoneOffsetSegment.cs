using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Abstractions.Services
{
    public interface ITimeZoneOffsetSegment
    {
        DateTime FromUtc { get; }
        /// <summary>
        /// FromIncluding: If true use greater than or equal to, otherwise just use greater than
        /// </summary>
        bool FromIncluding { get; }
        DateTime ToUtc { get; }
        /// <summary>
        /// ToIncluding: If true use less than or equal to, otherwise just use less than
        /// </summary>
        bool ToIncluding { get; }
        int HourOffset { get; }
    }
}
