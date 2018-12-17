using System;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
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

            return closeForDay > universeTime
                ? universeTime
                : closeForDay;
        }
    }
}
