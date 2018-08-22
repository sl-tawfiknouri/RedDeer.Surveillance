using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using TestHarness.Display;
using TestHarness.Display.Subscribers;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class StockExchangeStreamFactory : IStockExchangeStreamFactory
    {
        public IStockExchangeStream Create()
        {
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var exchangeStream = new StockExchangeStream(exchangeUnsubscriberFactory);

            return exchangeStream;
        }

        public IStockExchangeStream CreateDisplayable(IConsole console)
        {
            var exchangeStream = Create();
            var exchangeStreamDisplaySubscriber = new ExchangeFrameDisplaySubscriber(console);
            exchangeStream.Subscribe(exchangeStreamDisplaySubscriber);

            return exchangeStream;
        }
    }
}
