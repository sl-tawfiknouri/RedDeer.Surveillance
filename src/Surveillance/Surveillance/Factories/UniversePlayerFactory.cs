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
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;

        public UniversePlayerFactory(
            IUnsubscriberFactory<TradeOrderFrame> tradeUnsubscriberFactory,
            IUnsubscriberFactory<ExchangeFrame> exchangeUnsubscriberFactory,
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory)
        {
            _tradeFrameUnsubscriberFactory =
                tradeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(tradeUnsubscriberFactory));

            _exchangeFrameUnsubscriberFactory =
                exchangeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(exchangeUnsubscriberFactory));

            _universeEventUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
        }

        public IUniversePlayer Build()
        {
            return new UniversePlayer(
                _tradeFrameUnsubscriberFactory,
                _exchangeFrameUnsubscriberFactory,
                _universeEventUnsubscriberFactory);
        }
    }
}
