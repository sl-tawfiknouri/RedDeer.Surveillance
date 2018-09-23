using Domain.Trades.Orders;
using Utilities.Network_IO.Websocket_Hosts;
using NLog;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using TestHarness.Network_IO.Subscribers.Interfaces;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriber : BaseWebsocketSubscriber<TradeOrderFrame>, ITradeOrderWebsocketSubscriber
    {
        public TradeOrderWebsocketSubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory factory,
            ILogger logger) : base(networkSwitch, factory, logger)
        {
        }

        public override void OnNext(TradeOrderFrame value)
        {
            lock (StateLock)
            {
                var duplexedMessage = DuplexMessageFactory.Create(MessageType.ReddeerTradeFormat, value);
                NetworkSwitch.Send(duplexedMessage);
            }
        }
    }
}