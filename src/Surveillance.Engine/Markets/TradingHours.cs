namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    /// <summary>
    ///     Calculates the trading hours for a given date time
    ///     Has a four hour allowance before an opening before it considers a date to belong to a prior day
    /// </summary>
    public class TradingHours : ITradingHours
    {
        public TimeSpan CloseOffsetInUtc { get; set; }

        public bool IsValid { get; set; }

        public string Mic { get; set; }

        public TimeSpan OpenOffsetInUtc { get; set; }

        public DateTime ClosingInUtcForDay(DateTime universeTime)
        {
            var closeOnSameDay =
                new DateTime(universeTime.Year, universeTime.Month, universeTime.Day).Add(this.CloseOffsetInUtc);

            if (universeTime.TimeOfDay.Add(TimeSpan.FromHours(4)) >= this.OpenOffsetInUtc) return closeOnSameDay;
            return closeOnSameDay.Subtract(TimeSpan.FromDays(1));
        }

        public DateTime MinimumOfCloseInUtcForDayOrUniverse(DateTime universeTime)
        {
            var closeForDay = this.ClosingInUtcForDay(universeTime);
            var openForDay = this.OpeningInUtcForDay(universeTime);
            var adjustedUniverseDate = openForDay > universeTime ? openForDay : universeTime;

            return closeForDay > adjustedUniverseDate ? adjustedUniverseDate : closeForDay;
        }

        public DateTime OpeningInUtcForDay(DateTime universeTime)
        {
            var openOnSameDay =
                new DateTime(universeTime.Year, universeTime.Month, universeTime.Day).Add(this.OpenOffsetInUtc);
            if (universeTime.TimeOfDay.Add(TimeSpan.FromHours(4)) >= this.OpenOffsetInUtc) return openOnSameDay;
            return openOnSameDay.Subtract(TimeSpan.FromDays(1));
        }
    }
}