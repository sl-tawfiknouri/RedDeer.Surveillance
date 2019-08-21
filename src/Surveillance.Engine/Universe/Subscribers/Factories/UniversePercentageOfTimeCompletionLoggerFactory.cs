namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    public class UniversePercentageOfTimeCompletionLoggerFactory : IUniversePercentageOfTimeCompletionLoggerFactory
    {
        private readonly ILogger<UniversePercentageOfTimeCompletionLogger> _logger;

        public UniversePercentageOfTimeCompletionLoggerFactory(ILogger<UniversePercentageOfTimeCompletionLogger> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePercentageOfTimeCompletionLogger Build()
        {
            return new UniversePercentageOfTimeCompletionLogger(this._logger);
        }
    }
}