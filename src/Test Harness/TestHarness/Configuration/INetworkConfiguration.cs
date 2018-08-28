namespace TestHarness.Configuration
{
    public interface INetworkConfiguration
    {
        string TradeDomainUriDomainSegment { get; }
        string TradeDomainUriPort { get; }

        string StockExchangeDomainUriDomainSegment { get; }
        string StockExchangeDomainUriPort { get; }
    }
}
