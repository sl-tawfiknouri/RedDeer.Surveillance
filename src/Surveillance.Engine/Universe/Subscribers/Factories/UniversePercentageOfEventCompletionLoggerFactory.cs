namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    public class UniversePercentageOfEventCompletionLoggerFactory : IUniversePercentageOfEventCompletionLoggerFactory
    {
        private readonly ILogger<UniversePercentageOfEventCompletionLogger> _logger;

        public UniversePercentageOfEventCompletionLoggerFactory(
            ILogger<UniversePercentageOfEventCompletionLogger> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePercentageOfEventCompletionLogger Build()
        {
            return new UniversePercentageOfEventCompletionLogger(this._logger);
        }
    }
}