using System;
using DomainV2.Equity.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Factories
{
    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;

        public UniversePlayerFactory(IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory)
        {
            _universeEventUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
        }

        public IUniversePlayer Build()
        {
            return new UniversePlayer(_universeEventUnsubscriberFactory);
        }
    }
}
