using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class RefinitivTickPriceHistoryApiConfig 
        : IRefinitivTickPriceHistoryApiConfig
    {
        public string Address { get; set; }
    }
}
