using TestHarness.Configuration.Interfaces;

namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        /// <summary>
        /// Use defaults
        /// </summary>
        public Configuration()
        { }

        public string TradeDomainUriDomainSegment { get; } = "localhost";
        public string TradeDomainUriPort { get; } = "9067";

        public string StockExchangeDomainUriDomainSegment { get; } = "localhost";
        public string StockExchangeDomainUriPort { get; } = "9068";
    }
}
