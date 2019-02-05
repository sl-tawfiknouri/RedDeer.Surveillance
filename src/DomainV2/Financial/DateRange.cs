using System;

namespace DomainV2.Financial
{
    public class DateRange
    {
        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; }
        public DateTime End { get; }
        public TimeSpan Length => End.Subtract(Start);
    }
}
