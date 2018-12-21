using System;
using DataImport.Network_IO.Interfaces;
using DomainV2.Streams;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
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
        private readonly OrderStream<Order> _reddeerTradeFormatStream;
        private readonly ILogger<RelayTradeNetworkDuplexer> _logger;

        public RelayTradeNetworkDuplexer(
            OrderStream<Order> reddeerStream,
            ILogger<RelayTradeNetworkDuplexer> logger)
        {
            _reddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Transmit(IDuplexedMessage message)
        {
            if (message == null
                || message.Type == MessageType.Unknown)
            {
                _logger.LogInformation($"RelayTradeNetworkDuplexer received a message that was either null or had an unknown type.");
                return;
            }

            switch (message.Type)
            {
                case MessageType.ReddeerTradeFormat:
                    _logger.LogInformation($"RelayTradeNetworkDuplexer transmitting a trade message");
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
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            var formattedMessage = JsonConvert.DeserializeObject<Order>(message.Message, serialiserSettings);
            _reddeerTradeFormatStream?.Add(formattedMessage);
        }
    }
}
