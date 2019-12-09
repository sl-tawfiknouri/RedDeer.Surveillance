using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class RefinitivTickPriceHistoryApiConfig : IRefinitivTickPriceHistoryApiConfig
    {
        public string RefinitivTickPriceHistoryApiAddress { get; set; }
        public int RefinitivTickPriceHistoryApiPollingSeconds { get; set; } = 60;
        public int RefinitivTickPriceHistoryApiTimeOutDurationSeconds { get; set; } = 600;
    }
}
