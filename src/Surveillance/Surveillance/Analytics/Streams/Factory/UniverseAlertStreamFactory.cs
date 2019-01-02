using System;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Factory.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;

namespace Surveillance.Analytics.Streams.Factory
{
    public class UniverseAlertStreamFactory : IUniverseAlertStreamFactory
    {
        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _unsubscriberFactory;
        private readonly ILogger<UniverseAlertStream> _logger;

        public UniverseAlertStreamFactory(
            IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory,
            ILogger<UniverseAlertStream> logger)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseAlertStream Build()
        {
            return new UniverseAlertStream(_unsubscriberFactory, _logger);
        }
    }
}
