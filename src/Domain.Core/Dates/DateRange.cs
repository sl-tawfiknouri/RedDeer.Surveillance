namespace Domain.Core.Dates
{
    using System;

    public class DateRange
    {
        public DateRange(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;

            if (start > end) throw new ArgumentNullException(nameof(start));
        }

        public DateTime End { get; }

        public TimeSpan Length => this.End.Subtract(this.Start);

        public DateTime Start { get; }

        public bool Intersection(DateRange otherRange)
        {
            if (otherRange == null) return false;

            if (otherRange.Start <= this.Start && otherRange.End >= this.Start) return true;

            if (this.Start <= otherRange.Start && this.End >= otherRange.Start) return true;

            return false;
        }
    }
}