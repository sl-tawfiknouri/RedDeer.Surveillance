using DomainV2.Financial;
using DomainV2.Financial.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public DomainV2.Financial.Market Exchange { get; set; }
        public FinancialInstrument Security { get; set; }
    }
}