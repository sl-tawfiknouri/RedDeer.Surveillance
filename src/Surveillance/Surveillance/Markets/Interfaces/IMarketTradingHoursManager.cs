namespace Surveillance.Markets.Interfaces
{
    public interface IMarketTradingHoursManager
    {
        ITradingHours Get(string marketIdentifierCode);
    }
}