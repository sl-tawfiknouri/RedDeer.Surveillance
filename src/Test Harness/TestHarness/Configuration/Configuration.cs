using TestHarness.Configuration.Interfaces;

namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        /// <summary>
        /// Use defaults
        /// </summary>
        public Configuration()
        {
        }

        public string TradeWebsocketUriDomain { get; set; }
        public string TradeWebsocketUriPort { get; set; }

        public string StockExchangeDomainUriDomainSegment { get; set; }
        public string StockExchangeDomainUriPort { get; set; }
    }
}
