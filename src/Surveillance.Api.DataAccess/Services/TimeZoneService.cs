namespace Surveillance.Api.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Surveillance.Api.DataAccess.Abstractions.Services;

    using TimeZoneConverter;

    public class TimeZoneService : ITimeZoneService
    {
        public IEnumerable<ITimeZoneOffsetSegment> GetOffsetSegments(DateTime fromUtc, DateTime toUtc, string tzName)
        {
            var timeZone = this.GetTimeZone(tzName);
            var toLocal = TimeZoneInfo.ConvertTimeFromUtc(toUtc, timeZone);

            var segments = new List<TimeZoneOffsetSegment>();
            var from = fromUtc;
            var nextCheck = from;

            var count = 0;
            while (true)
            {
                var (nextLocal, nextDelta) = this.GetNextTransition(
                    TimeZoneInfo.ConvertTimeFromUtc(nextCheck, timeZone),
                    timeZone);
                DateTime next;

                var isLast = nextLocal == null || nextLocal > toLocal;

                if (isLast)
                {
                    next = toUtc;
                }
                else
                {
                    next = TimeZoneInfo.ConvertTimeToUtc(nextLocal.Value + nextDelta.Value, timeZone);
                    if (timeZone.IsInvalidTime(nextLocal.Value)) nextCheck = next;
                    else nextCheck = TimeZoneInfo.ConvertTimeToUtc(nextLocal.Value, timeZone).AddMilliseconds(1);
                }

                segments.Add(
                    new TimeZoneOffsetSegment
                        {
                            FromUtc = from,
                            ToUtc = next,
                            FromIncluding = true,
                            ToIncluding = isLast,
                            HourOffset = this.HoursOffset(from, timeZone)
                        });

                if (isLast) break;

                from = next;

                count++;
                if (count > 10000) throw new Exception("Too many iterations");
            }

            return segments;
        }

        // https://stackoverflow.com/questions/4764743/get-the-next-date-time-at-which-a-daylight-savings-time-transition-occurs
        private DateTime GetAdjustmentDate(TimeZoneInfo.TransitionTime transitionTime, int year)
        {
            if (transitionTime.IsFixedDateRule) return new DateTime(year, transitionTime.Month, transitionTime.Day);

            // For non-fixed date rules, get local calendar
            var cal = CultureInfo.CurrentCulture.Calendar;

            // Get first day of week for transition
            // For example, the 3rd week starts no earlier than the 15th of the month
            var startOfWeek = transitionTime.Week * 7 - 6;

            // What day of the week does the month start on?
            var firstDayOfWeek = (int)cal.GetDayOfWeek(new DateTime(year, transitionTime.Month, 1));

            // Determine how much start date has to be adjusted
            int transitionDay;
            var changeDayOfWeek = (int)transitionTime.DayOfWeek;

            if (firstDayOfWeek <= changeDayOfWeek)
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
            else
                transitionDay = startOfWeek + (7 - firstDayOfWeek) + changeDayOfWeek;

            // Adjust for months with no fifth week
            if (transitionDay > cal.GetDaysInMonth(year, transitionTime.Month))
                transitionDay -= 7;

            return new DateTime(
                year,
                transitionTime.Month,
                transitionDay,
                transitionTime.TimeOfDay.Hour,
                transitionTime.TimeOfDay.Minute,
                transitionTime.TimeOfDay.Second);
        }

        // https://stackoverflow.com/questions/4764743/get-the-next-date-time-at-which-a-daylight-savings-time-transition-occurs
        private (DateTime?, TimeSpan?) GetNextTransition(DateTime asOfTime, TimeZoneInfo timeZone)
        {
            var adjustments = timeZone.GetAdjustmentRules();
            if (adjustments.Length == 0) return (null, null);

            var year = asOfTime.Year;
            TimeZoneInfo.AdjustmentRule adjustment = null;
            foreach (var adj in adjustments)

                // Determine if this adjustment rule covers year desired
                if (adj.DateStart.Year <= year && adj.DateEnd.Year >= year)
                {
                    adjustment = adj;
                    break;
                }

            if (adjustment == null) return (null, null);

            var dtAdjustmentStart = this.GetAdjustmentDate(adjustment.DaylightTransitionStart, year);
            var dtAdjustmentEnd = this.GetAdjustmentDate(adjustment.DaylightTransitionEnd, year);

            if (dtAdjustmentStart >= asOfTime) return (dtAdjustmentStart, adjustment.DaylightDelta);

            if (dtAdjustmentEnd >= asOfTime)
            {
                // otherwise adjustment end date should be the next transition date
                return (dtAdjustmentEnd, -adjustment.DaylightDelta);
            }

            // then it should be the next year's DaylightTransitionStart
            year++;
            foreach (var adj in adjustments)

                // Determine if this adjustment rule covers year desired
                if (adj.DateStart.Year <= year && adj.DateEnd.Year >= year)
                {
                    adjustment = adj;
                    break;
                }

            dtAdjustmentStart = this.GetAdjustmentDate(adjustment.DaylightTransitionStart, year);
            return (dtAdjustmentStart, adjustment.DaylightDelta);
        }

        private TimeZoneInfo GetTimeZone(string tzName)
        {
            var timeZone = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                               ? TimeZoneInfo.FindSystemTimeZoneById(tzName)
                               : TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(tzName));

            if (timeZone == null)
                throw new ApplicationException("No timeZone found");

            return timeZone;
        }

        private int HoursOffset(DateTime dateTime, TimeZoneInfo timeZone)
        {
            return Convert.ToInt32((TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone) - dateTime).TotalHours);
        }
    }
}