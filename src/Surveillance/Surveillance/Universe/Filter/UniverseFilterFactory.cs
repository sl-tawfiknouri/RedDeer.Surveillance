using System;
using DomainV2.Equity.Streams.Interfaces;
using Surveillance.RuleParameters.Filter;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter
{
    public class UniverseFilterFactory : IUniverseFilterFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;

        public UniverseFilterFactory(IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IUniverseFilter Build(
                RuleFilter accounts,
                RuleFilter traders,
                RuleFilter markets)
        {
            return new UniverseFilter(_unsubscriberFactory, accounts, traders, markets);
        }
    }
}
