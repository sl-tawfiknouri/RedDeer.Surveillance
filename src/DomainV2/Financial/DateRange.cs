using System;

namespace Domain.Financial
{
    public class DateRange
    {
        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;

            if (start > end)
            {
                throw new ArgumentNullException(nameof(start));
            }
        }

        public DateTime Start { get; }
        public DateTime End { get; }
        public TimeSpan Length => End.Subtract(Start);

        public bool Intersection(DateRange otherRange)
        {
            if (otherRange == null)
            {
                return false;
            }

            if (otherRange.Start <= Start
                && otherRange.End >= Start)
            {
                return true;
            }

            if (Start <= otherRange.Start
                && End >= otherRange.Start)
            {
                return true;
            }

            return false;
        }
    }
}
