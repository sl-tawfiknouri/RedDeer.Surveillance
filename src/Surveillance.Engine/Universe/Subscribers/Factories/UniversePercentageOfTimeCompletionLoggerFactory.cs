namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    /// The universe percentage of time completion logger factory.
    /// </summary>
    public class UniversePercentageOfTimeCompletionLoggerFactory : IUniversePercentageOfTimeCompletionLoggerFactory
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniversePercentageOfTimeCompletionLogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversePercentageOfTimeCompletionLoggerFactory"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniversePercentageOfTimeCompletionLoggerFactory(ILogger<UniversePercentageOfTimeCompletionLogger> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniversePercentageOfTimeCompletionLogger"/>.
        /// </returns>
        public IUniversePercentageOfTimeCompletionLogger Build()
        {
            return new UniversePercentageOfTimeCompletionLogger(this.logger);
        }
    }
}