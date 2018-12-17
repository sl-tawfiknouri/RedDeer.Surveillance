using System;
using Microsoft.Extensions.Logging;
using Surveillance.Universe.Subscribers.Interfaces;

namespace Surveillance.Universe.Subscribers
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
