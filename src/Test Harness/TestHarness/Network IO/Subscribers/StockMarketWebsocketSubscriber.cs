using Domain.Equity.Trading.Frames;
using NLog;
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
            var duplexedMessage = _duplexMessageFactory.Create(MessageType.RedderStockFormat, value);
            _networkSwitch.Send(duplexedMessage);
        }
    }
}
