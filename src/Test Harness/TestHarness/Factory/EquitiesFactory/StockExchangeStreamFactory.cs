using Domain.Core.Markets.Collections;
using Domain.Surveillance.Streams;
using Domain.Surveillance.Streams.Interfaces;
using TestHarness.Display.Interfaces;
using TestHarness.Display.Subscribers;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class StockExchangeStreamFactory : IStockExchangeStreamFactory
    {
        public IStockExchangeStream Create()
        {
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<EquityIntraDayTimeBarCollection>();
            var exchangeStream = new ExchangeStream(exchangeUnsubscriberFactory);

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
