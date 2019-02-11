﻿using System;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    /// <summary>
    /// Calculates the trading hours for a given date time
    /// Has a four hour allowance before an opening before it considers a date to belong to a prior day
    /// </summary>
    public class TradingHours : ITradingHours
    {
        public string Mic { get; set; }
        public bool IsValid { get; set; }
        public TimeSpan OpenOffsetInUtc { get; set; }
        public TimeSpan CloseOffsetInUtc { get; set; }

        public DateTime OpeningInUtcForDay(DateTime universeTime)
        {
            var openOnSameDay = new DateTime(universeTime.Year, universeTime.Month, universeTime.Day).Add(OpenOffsetInUtc);
            if (universeTime.TimeOfDay.Add(TimeSpan.FromHours(4)) >= OpenOffsetInUtc)
            {
                return openOnSameDay;
            }
            else
            {
                return openOnSameDay.Subtract(TimeSpan.FromDays(1));
            }
        }

        public DateTime ClosingInUtcForDay(DateTime universeTime)
        {
            var closeOnSameDay =
                new DateTime(universeTime.Year, universeTime.Month, universeTime.Day)
                    .Add(CloseOffsetInUtc);

            if (universeTime.TimeOfDay.Add(TimeSpan.FromHours(4)) >= OpenOffsetInUtc)
            {
                return closeOnSameDay;
            }
            else
            {
                return closeOnSameDay.Subtract(TimeSpan.FromDays(1));
            }
        }

        public DateTime MinimumOfCloseInUtcForDayOrUniverse(DateTime universeTime)
        {
            var closeForDay = ClosingInUtcForDay(universeTime);
            var openForDay = OpeningInUtcForDay(universeTime);
            var adjustedUniverseDate = openForDay > universeTime ? openForDay : universeTime;

            return closeForDay > adjustedUniverseDate
                ? adjustedUniverseDate
                : closeForDay;
        }
    }
}
