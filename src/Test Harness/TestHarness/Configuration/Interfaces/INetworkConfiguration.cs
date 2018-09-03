namespace TestHarness.Configuration.Interfaces
{
    public interface INetworkConfiguration
    {
        string TradeWebsocketUriDomain { get; }
        string TradeWebsocketUriPort { get; }

        string StockExchangeDomainUriDomainSegment { get; }
        string StockExchangeDomainUriPort { get; }
    }
}
