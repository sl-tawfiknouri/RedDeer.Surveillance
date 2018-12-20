using System;
    
namespace Surveillance.Markets.Interfaces
{
    public interface ITradingHours
    {
        TimeSpan CloseOffsetInUtc { get; set; }
        bool IsValid { get; set; }
        string Mic { get; set; }
        TimeSpan OpenOffsetInUtc { get; set; }

        DateTime ClosingInUtcForDay(DateTime universeTime);
        DateTime MinimumOfCloseInUtcForDayOrUniverse(DateTime universeTime);
        DateTime OpeningInUtcForDay(DateTime universeTime);
    }
}