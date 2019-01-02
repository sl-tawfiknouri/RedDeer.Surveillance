using System;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Factories
{
    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;
        private readonly ILogger<UniversePlayer> _logger;

        public UniversePlayerFactory(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            _universeEventUnsubscriberFactory =
                universeUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePlayer Build()
        {
            return new UniversePlayer(_universeEventUnsubscriberFactory, _logger);
        }
    }
}
