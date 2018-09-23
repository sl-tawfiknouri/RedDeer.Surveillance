using Newtonsoft.Json;
using Relay.Network_IO.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Network_IO
{
    public class RelayEquityNetworkDuplexer : IRelayEquityNetworkDuplexer
    {
        private readonly IStockExchangeStream _redeerStockFormatStream;

        public RelayEquityNetworkDuplexer(IStockExchangeStream reddeerStockFormatStream)
        {
            _redeerStockFormatStream = 
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
                default:
                    break;
            }
        }

        private void ReddeerStockExchangeFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<ExchangeFrame>(message.Message, serialiserSettings);
            _redeerStockFormatStream?.Add(formattedMessage);
        }
    }
}
