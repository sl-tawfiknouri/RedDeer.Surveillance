using System;
using Domain.Surveillance.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class UniverseFilterFactory : IUniverseFilterFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ILogger<UniverseFilterService> _logger;

        public UniverseFilterFactory(
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ILogger<UniverseFilterService> logger)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseFilterService Build(
                RuleFilter accounts,
                RuleFilter traders,
                RuleFilter markets,
                RuleFilter funds,
                RuleFilter strategies)
        {
            return new UniverseFilterService(_unsubscriberFactory, accounts, traders, markets, funds, strategies, _logger);
        }
    }
}
