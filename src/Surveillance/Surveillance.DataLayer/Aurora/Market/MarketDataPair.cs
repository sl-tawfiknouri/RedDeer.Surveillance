namespace Surveillance.DataLayer.Aurora.Market
{
    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets;

    public class MarketDataPair
    {
        public Market Exchange { get; set; }

        public FinancialInstrument Security { get; set; }
    }
}