namespace TestHarness.Factory.TradingFactory
{
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams;
    using Domain.Surveillance.Streams.Interfaces;

    using TestHarness.Display.Interfaces;
    using TestHarness.Display.Subscribers;
    using TestHarness.Factory.TradingFactory.Interfaces;

    public class TradeOrderStreamFactory : ITradeOrderStreamFactory
    {
        public IOrderStream<Order> Create()
        {
            var tradeUnsubscriberFactory = new UnsubscriberFactory<Order>();
            var tradeOrderStream = new OrderStream<Order>(tradeUnsubscriberFactory);

            return tradeOrderStream;
        }

        public IOrderStream<Order> CreateDisplayable(IConsole console)
        {
            var tradeOrderStream = this.Create();
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(console);
            tradeOrderStream.Subscribe(tradeOrderDisplaySubscriber);

            return tradeOrderStream;
        }
    }
}