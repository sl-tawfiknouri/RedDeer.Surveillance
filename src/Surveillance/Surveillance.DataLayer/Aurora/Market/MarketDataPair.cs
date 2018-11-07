using Domain.Equity;
using Domain.Market;

namespace Surveillance.DataLayer.Aurora.Market
{
    public class MarketDataPair
    {
        public StockExchange Exchange { get; set; }
        public Security Security { get; set; }
    }
}