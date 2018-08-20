namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        public Configuration(
            string tradeDomainUriSegment,
            string tradeDomainUriPort)
        {
            TradeDomainUriDomainSegment = tradeDomainUriSegment ?? string.Empty;
            TradeDomainUriPort = tradeDomainUriPort ?? string.Empty;
        }

        public string TradeDomainUriDomainSegment { get; }
        public string TradeDomainUriPort { get; }
    }
}
