using DomainV2.Equity.Frames;
using Microsoft.Extensions.Logging;
using TestHarness.Network_IO.Subscribers.Interfaces;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace TestHarness.Network_IO.Subscribers
{
    public class StockMarketWebsocketSubscriber : BaseWebsocketSubscriber<ExchangeFrame>, IStockMarketWebsocketSubscriber
    {
        public StockMarketWebsocketSubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory factory,
            ILogger logger) : base(networkSwitch, factory, logger)
        { }

        public override void OnNext(ExchangeFrame value)
        {
            var duplexedMessage = DuplexMessageFactory.Create(MessageType.RedderStockFormat, value);
            NetworkSwitch.Send(duplexedMessage);
        }
    }
}
