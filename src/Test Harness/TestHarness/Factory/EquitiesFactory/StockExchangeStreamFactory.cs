using DomainV2.Equity.Streams;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Streams;
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
