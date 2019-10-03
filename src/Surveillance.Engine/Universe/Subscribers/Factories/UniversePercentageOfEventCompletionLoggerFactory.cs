namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    /// The universe percentage of event completion logger factory.
    /// </summary>
    public class UniversePercentageOfEventCompletionLoggerFactory : IUniversePercentageOfEventCompletionLoggerFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePercentageOfEventCompletionLogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageOfEventCompletionLoggerFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePercentageOfEventCompletionLoggerFactory(
            ILogger<UniversePercentageOfEventCompletionLogger> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageOfEventCompletionLogger"/>.
        /// </returns>
        public IUniversePercentageOfEventCompletionLogger Build()
        {
            return new UniversePercentageOfEventCompletionLogger(this.logger);
        }
    }
}