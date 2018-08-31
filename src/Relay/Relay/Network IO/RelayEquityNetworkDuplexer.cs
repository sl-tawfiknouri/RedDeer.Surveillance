using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using Newtonsoft.Json;
using Relay.Network_IO.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Network_IO
{
    public class RelayEquityNetworkDuplexer : IRelayEquityNetworkDuplexer
    {
        private IStockExchangeStream _RedeerStockFormatStream;

        public RelayEquityNetworkDuplexer(IStockExchangeStream reddeerStockFormatStream)
        {
            _RedeerStockFormatStream = 
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
            _RedeerStockFormatStream?.Add(formattedMessage);
        }
    }
}
