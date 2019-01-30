using DomainV2.Financial;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public DomainV2.Financial.Market Exchange { get; set; }
        public FinancialInstrument Security { get; set; }
    }
}