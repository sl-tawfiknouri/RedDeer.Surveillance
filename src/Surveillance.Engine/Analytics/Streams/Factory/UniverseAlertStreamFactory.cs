namespace Surveillance.Engine.Rules.Analytics.Streams.Factory
{
    using System;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    public class UniverseAlertStreamFactory : IUniverseAlertStreamFactory
    {
        private readonly ILogger<UniverseAlertStream> _logger;

        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _unsubscriberFactory;

        public UniverseAlertStreamFactory(
            IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory,
            ILogger<UniverseAlertStream> logger)
        {
            this._unsubscriberFactory =
                unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseAlertStream Build()
        {
            return new UniverseAlertStream(this._unsubscriberFactory, this._logger);
        }
    }
}