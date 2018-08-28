using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Newtonsoft.Json;
using System;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Network_IO
{
    public class SurveillanceNetworkDuplexer : INetworkDuplexer
    {
        private ITradeOrderStream<TradeOrderFrame> _ReddeerTradeFormatStream;

        public SurveillanceNetworkDuplexer(ITradeOrderStream<TradeOrderFrame> reddeerStream)
        {
            _ReddeerTradeFormatStream = reddeerStream ?? throw new ArgumentNullException(nameof(reddeerStream));
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
                default:
                    break;
            }
        }

        private void ReddeerFormat(IDuplexedMessage message)
        {
            var formattedMessage = JsonConvert.DeserializeObject<TradeOrderFrame>(message.Message);
            _ReddeerTradeFormatStream?.Add(formattedMessage);
        }

        private void FixFormat(IDuplexedMessage message)
        {

        }
    }
}
