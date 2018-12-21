using System;
using DataImport.Network_IO.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Network_IO
{
    public class RelayEquityNetworkDuplexer : IRelayEquityNetworkDuplexer
    {
        private readonly IStockExchangeStream _redDeerStockFormatStream;
        private readonly ILogger<RelayEquityNetworkDuplexer> _logger;

        public RelayEquityNetworkDuplexer(
            IStockExchangeStream reddeerStockFormatStream,
            ILogger<RelayEquityNetworkDuplexer> logger)
        {
            _redDeerStockFormatStream = 
                reddeerStockFormatStream 
                ?? throw new ArgumentNullException(nameof(reddeerStockFormatStream));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Transmit(IDuplexedMessage message)
        {
            if (message == null 
                || message.Type == MessageType.Unknown)
            {
                _logger.LogError($"RelayEquityNetworkDuplexer received a null message or a message of type unknown");
                return;
            }

            switch (message.Type)
            {
                case MessageType.RedderStockFormat:
                    _logger.LogInformation($"RelayEquityNetworkDuplexer received a message of type stock data");
                    ReddeerStockExchangeFormat(message);
                    break;
                case MessageType.Unknown:
                    break;
                case MessageType.ReddeerTradeFormat:
                    break;
                case MessageType.FixTradeFormat:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ReddeerStockExchangeFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<ExchangeFrame>(message.Message, serialiserSettings);
            _redDeerStockFormatStream?.Add(formattedMessage);
        }
    }
}
