using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using TestHarness.Network_IO.Subscribers.Interfaces;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriber : BaseWebsocketSubscriber<Order>, ITradeOrderWebsocketSubscriber
    {
        public TradeOrderWebsocketSubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory factory,
            ILogger logger) : base(networkSwitch, factory, logger)
        {
        }

        public override void OnNext(Order value)
        {
            lock (StateLock)
            {
                var duplexedMessage = DuplexMessageFactory.Create(MessageType.ReddeerTradeFormat, value);
                NetworkSwitch.Send(duplexedMessage);
            }
        }
    }
}