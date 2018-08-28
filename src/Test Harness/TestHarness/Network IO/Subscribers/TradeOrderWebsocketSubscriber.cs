using Utilities.Network_IO.Websocket_Hosts;
using Domain.Equity.Trading.Orders;
using NLog;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

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
            lock (_stateLock)
            {
                var duplexedMessage = _duplexMessageFactory.Create(MessageType.ReddeerTradeFormat, value);
                _networkSwitch.Send(duplexedMessage);
            }
        }
    }
}