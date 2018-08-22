using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using TestHarness.Display;
using TestHarness.Display.Subscribers;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class TradeOrderStreamFactory : ITradeOrderStreamFactory
    {
        public ITradeOrderStream Create()
        {
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);

            return tradeOrderStream;
        }

        public ITradeOrderStream CreateDisplayable(IConsole console)
        {
            var tradeOrderStream = Create();
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(console);
            tradeOrderStream.Subscribe(tradeOrderDisplaySubscriber);

            return tradeOrderStream;
        }
    }
}