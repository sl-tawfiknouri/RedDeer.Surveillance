using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Surveillance.Factories.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Factories
{
    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        private readonly IUnsubscriberFactory<TradeOrderFrame> _tradeFrameUnsubscriberFactory;
        private readonly IUnsubscriberFactory<ExchangeFrame> _exchangeFrameUnsubscriberFactory;

        public UniversePlayerFactory(
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> exchangeFactory)
        {
            _tradeFrameUnsubscriberFactory =
                unsubscriberFactory
                ?? throw new ArgumentNullException(nameof(unsubscriberFactory));

            _exchangeFrameUnsubscriberFactory =
                exchangeFactory
                ?? throw new ArgumentNullException(nameof(exchangeFactory));
        }

        public IUniversePlayer Build()
        {
            return new UniversePlayer(_tradeFrameUnsubscriberFactory, _exchangeFrameUnsubscriberFactory);
        }
    }
}
