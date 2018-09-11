using Utilities.Aws_IO.Interfaces;

namespace TestHarness.Configuration.Interfaces
{
    public interface INetworkConfiguration : IAwsConfiguration
    {
        string TradeWebsocketUriDomain { get; }
        string TradeWebsocketUriPort { get; }

        string StockExchangeDomainUriDomainSegment { get; }
        string StockExchangeDomainUriPort { get; }
    }
}
