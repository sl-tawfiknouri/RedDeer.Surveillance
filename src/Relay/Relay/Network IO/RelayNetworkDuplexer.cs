using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Network_IO
{
    public class RelayNetworkDuplexer : INetworkDuplexer
    {
        private ITradeOrderStream<TradeOrderFrame> _ReddeerTradeFormatStream;

        public RelayNetworkDuplexer(ITradeOrderStream<TradeOrderFrame> reddeerStream)
        {
            _ReddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
        }

        public void Transmit(IDuplexedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
