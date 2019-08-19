namespace Surveillance.Engine.Rules.Factories
{
    using System;
    using System.Threading;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        private readonly ILogger<UniversePlayer> _logger;

        private readonly IUnsubscriberFactory<IUniverseEvent> _universeEventUnsubscriberFactory;

        public UniversePlayerFactory(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            this._universeEventUnsubscriberFactory = universeUnsubscriberFactory
                                                     ?? throw new ArgumentNullException(
                                                         nameof(universeUnsubscriberFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePlayer Build(CancellationToken ct)
        {
            return new UniversePlayer(ct, this._universeEventUnsubscriberFactory, this._logger);
        }
    }
}