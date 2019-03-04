using Domain.Core.Financial;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public Domain.Core.Financial.Markets.Market Exchange { get; set; }
        public FinancialInstrument Security { get; set; }
    }
}