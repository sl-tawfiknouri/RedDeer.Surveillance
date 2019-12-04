
namespace Surveillance.Data.Universe.Refinitiv.Interfaces
{
    public interface IRefinitivTickPriceHistoryApiConfig
    {
        string RefinitivTickPriceHistoryApiAddress { get; set; }
        int RefinitivTickPriceHistoryApiPollingSeconds { get; set; }
        int RefinitivTickPriceHistoryApiTimeOutDurationSeconds { get; set; }
    }
}
