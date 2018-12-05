using System;
using DataImport.Network_IO.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams.Interfaces;
using Newtonsoft.Json;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Network_IO
{
    public class RelayEquityNetworkDuplexer : IRelayEquityNetworkDuplexer
    {
        private readonly IStockExchangeStream _redDeerStockFormatStream;

        public RelayEquityNetworkDuplexer(IStockExchangeStream reddeerStockFormatStream)
        {
            _redDeerStockFormatStream = 
                reddeerStockFormatStream 
                ?? throw new ArgumentNullException(nameof(reddeerStockFormatStream));
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
                case MessageType.RedderStockFormat:
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
