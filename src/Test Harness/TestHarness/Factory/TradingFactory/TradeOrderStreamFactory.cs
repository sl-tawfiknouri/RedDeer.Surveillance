using Domain.Streams;
using Domain.Trades.Orders;
using Domain.Trades.Streams;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Display;
using TestHarness.Display.Subscribers;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class TradeOrderStreamFactory : ITradeOrderStreamFactory
    {
        public ITradeOrderStream<TradeOrderFrame> Create()
        {
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeOrderStream = new TradeOrderStream<TradeOrderFrame>(tradeUnsubscriberFactory);

            return tradeOrderStream;
        }

        public ITradeOrderStream<TradeOrderFrame> CreateDisplayable(IConsole console)
        {
            var tradeOrderStream = Create();
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(console);
            tradeOrderStream.Subscribe(tradeOrderDisplaySubscriber);

            return tradeOrderStream;
        }
    }
}