using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Newtonsoft.Json;
using Relay.Network_IO.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Relay.Network_IO
{
    /// <summary>
    /// deserialises incoming duplexed messages and forwards them onto subscribing streams
    /// </summary>
    public class RelayTradeNetworkDuplexer : IRelayTradeNetworkDuplexer
    {
        private ITradeOrderStream<TradeOrderFrame> _ReddeerTradeFormatStream;

        public RelayTradeNetworkDuplexer(ITradeOrderStream<TradeOrderFrame> reddeerStream)
        {
            _ReddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
        }

        public void Transmit(IDuplexedMessage message)
        {
            if (message == null
                || message.Type == MessageType.Unknown)
            {
                return;
            }

            switch (message.Type)
            {
                case MessageType.ReddeerTradeFormat:
                    ReddeerTradeFormat(message);
                    break;
                case MessageType.FixTradeFormat:
                    FixFormat(message);
                    break;
                default:
                    break;
            }
        }

        private void ReddeerTradeFormat(IDuplexedMessage message)
        {
            var formattedMessage = JsonConvert.DeserializeObject<TradeOrderFrame>(message.Message);
            _ReddeerTradeFormatStream?.Add(formattedMessage);
        }

        private void FixFormat(IDuplexedMessage message)
        {

        }
    }
}
