using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    public class UniversePercentageOfTimeCompletionLoggerFactory : IUniversePercentageOfTimeCompletionLoggerFactory
    {
        private readonly ILogger<UniversePercentageOfTimeCompletionLogger> _logger;

        public UniversePercentageOfTimeCompletionLoggerFactory(ILogger<UniversePercentageOfTimeCompletionLogger> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePercentageOfTimeCompletionLogger Build()
        {
            return new UniversePercentageOfTimeCompletionLogger(_logger);
        }
    }
}
