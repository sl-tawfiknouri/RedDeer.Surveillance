using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using Newtonsoft.Json;
using Surveillance.Network_IO.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Network_IO
{
    public class SurveillanceEquityNetworkDuplexer : ISurveillanceEquityNetworkDuplexer
    {
        private IStockExchangeStream _ReddeerStockExchangeStream; 

        public SurveillanceEquityNetworkDuplexer(IStockExchangeStream stockExchangeStream)
        {
            _ReddeerStockExchangeStream = 
                stockExchangeStream 
                ?? throw new ArgumentNullException(nameof(stockExchangeStream));
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
                    ReddeerStockFormat(message);
                    break;
                default:
                    break;
            }
        }

        private void ReddeerStockFormat(IDuplexedMessage message)
        {
            var formattedMessage = JsonConvert.DeserializeObject<ExchangeFrame>(message.Message);
            _ReddeerStockExchangeStream?.Add(formattedMessage);
        }
    }
}
