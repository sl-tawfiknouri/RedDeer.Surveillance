using Domain.Core.Financial.Assets;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public Domain.Core.Markets.Market Exchange { get; set; }
        public FinancialInstrument Security { get; set; }
    }
}