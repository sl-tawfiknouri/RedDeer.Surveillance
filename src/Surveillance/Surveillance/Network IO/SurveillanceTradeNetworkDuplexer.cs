using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Newtonsoft.Json;
using Surveillance.Network_IO.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Network_IO
{
    public class SurveillanceNetworkDuplexer : ISurveillanceNetworkDuplexer
    {
        private ITradeOrderStream<TradeOrderFrame> _ReddeerTradeFormatStream;
        private IStockExchangeStream _ReddeerStockExchangeStream;

        public SurveillanceNetworkDuplexer(
            ITradeOrderStream<TradeOrderFrame> reddeerStream,
            IStockExchangeStream stockExchangeStream)
        {
            _ReddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
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
                case MessageType.ReddeerTradeFormat:
                    ReddeerFormat(message);
                    break;
                case MessageType.FixTradeFormat:
                    break;
                case MessageType.RedderStockFormat:
                    ReddeerStockFormat(message);
                    break;
                default:
                    break;
            }
        }

        private void ReddeerFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<TradeOrderFrame>(message.Message, serialiserSettings);
            _ReddeerTradeFormatStream?.Add(formattedMessage);
        }

        private void FixFormat(IDuplexedMessage message)
        {

        }

        private void ReddeerStockFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<ExchangeFrame>(message.Message, serialiserSettings);
            _ReddeerStockExchangeStream?.Add(formattedMessage);
        }
    }
}
