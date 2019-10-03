namespace Surveillance.Engine.Rules.Factories
{
    using System;
    using System.Threading;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;

    /// <summary>
    /// The universe player factory.
    /// </summary>
    public class UniversePlayerFactory : IUniversePlayerFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePlayer> logger;

        /// <summary>
        /// The universe event factory.
        /// </summary>
        private readonly IUnsubscriberFactory<IUniverseEvent> universeEventUnsubscriberFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePlayerFactory"/> class.
        /// </summary>
        /// <param name="universeUnsubscriberFactory">
        /// The universe factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePlayerFactory(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            ILogger<UniversePlayer> logger)
        {
            this.universeEventUnsubscriberFactory =
                universeUnsubscriberFactory ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The build a universe player.
        /// wrapper to prevent hard coded dependencies
        /// </summary>
        /// <param name="ct">
        /// The ct.
        /// </param>
        /// <returns>
        /// The <see cref="IUniversePlayer"/>.
        /// </returns>
        public IUniversePlayer Build(CancellationToken ct)
        {
            return new UniversePlayer(ct, this.universeEventUnsubscriberFactory, this.logger);
        }
    }
}