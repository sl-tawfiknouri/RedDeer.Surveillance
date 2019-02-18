using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Factories
{
    public class UniversePercentageOfEventCompletionLoggerFactory : IUniversePercentageOfEventCompletionLoggerFactory
    {
        private readonly ILogger<UniversePercentageOfEventCompletionLogger> _logger;

        public UniversePercentageOfEventCompletionLoggerFactory(ILogger<UniversePercentageOfEventCompletionLogger> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniversePercentageOfEventCompletionLogger Build()
        {
            return new UniversePercentageOfEventCompletionLogger(_logger);
        }
    }
}
