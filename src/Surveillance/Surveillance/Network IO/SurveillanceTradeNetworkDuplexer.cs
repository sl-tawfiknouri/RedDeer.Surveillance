using Newtonsoft.Json;
using Surveillance.Network_IO.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Network_IO
{
    public class SurveillanceNetworkDuplexer : ISurveillanceNetworkDuplexer
    {
        private readonly ITradeOrderStream<TradeOrderFrame> _reddeerTradeFormatStream;
        private readonly IStockExchangeStream _reddeerStockExchangeStream;

        public SurveillanceNetworkDuplexer(
            ITradeOrderStream<TradeOrderFrame> reddeerStream,
            IStockExchangeStream stockExchangeStream)
        {
            _reddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
            _reddeerStockExchangeStream =
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
                case MessageType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ReddeerFormat(IDuplexedMessage message)
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

        private void ReddeerStockFormat(IDuplexedMessage message)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            var formattedMessage = JsonConvert.DeserializeObject<ExchangeFrame>(message.Message, serialiserSettings);
            _reddeerStockExchangeStream?.Add(formattedMessage);
        }
    }
}
