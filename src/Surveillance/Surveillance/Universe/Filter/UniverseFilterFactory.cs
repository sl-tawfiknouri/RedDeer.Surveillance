using System;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.RuleParameters.Filter;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter
{
    public class UniverseFilterFactory : IUniverseFilterFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ILogger<UniverseFilter> _logger;

        public UniverseFilterFactory(
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ILogger<UniverseFilter> logger)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseFilter Build(
                RuleFilter accounts,
                RuleFilter traders,
                RuleFilter markets)
        {
            return new UniverseFilter(_unsubscriberFactory, accounts, traders, markets, _logger);
        }
    }
}
