using Domain.Financial;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public Domain.Financial.Market Exchange { get; set; }
        public FinancialInstrument Security { get; set; }
    }
}