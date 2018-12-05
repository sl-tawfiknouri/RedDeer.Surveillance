using System;
using DataImport.Network_IO.Interfaces;
using Domain.Trades.Streams.Interfaces;
using DomainV2.Trading;
using Newtonsoft.Json;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Network_IO
{
    /// <summary>
    /// Deserialises incoming duplexed messages and forwards them onto subscribing streams
    /// </summary>
    public class RelayTradeNetworkDuplexer : IRelayTradeNetworkDuplexer
    {
        private readonly ITradeOrderStream<Order> _reddeerTradeFormatStream;

        public RelayTradeNetworkDuplexer(ITradeOrderStream<Order> reddeerStream)
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
                    break;
                case MessageType.Unknown:
                    break;
                case MessageType.RedderStockFormat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ReddeerTradeFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<Order>(message.Message, serialiserSettings);
            _reddeerTradeFormatStream?.Add(formattedMessage);
        }
    }
}
