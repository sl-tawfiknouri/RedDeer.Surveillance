using System;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Streams.Factory
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
