using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Surveillance.Factories.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Factories
{
    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        private readonly IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;

        public UniversePlayerFactory(IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IUniversePlayer Build()
        {
            return new UniversePlayer(_unsubscriberFactory);
        }
    }
}
