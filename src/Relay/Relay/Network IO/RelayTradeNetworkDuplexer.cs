using Newtonsoft.Json;
using Relay.Network_IO.Interfaces;
using System;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Network_IO
{
    /// <summary>
    /// deserialises incoming duplexed messages and forwards them onto subscribing streams
    /// </summary>
    public class RelayTradeNetworkDuplexer : IRelayTradeNetworkDuplexer
    {
        private readonly ITradeOrderStream<TradeOrderFrame> _reddeerTradeFormatStream;

        public RelayTradeNetworkDuplexer(ITradeOrderStream<TradeOrderFrame> reddeerStream)
        {
            _reddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
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
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<TradeOrderFrame>(message.Message, serialiserSettings);
            _reddeerTradeFormatStream?.Add(formattedMessage);
        }

        private void FixFormat(IDuplexedMessage message)
        {

        }
    }
}
