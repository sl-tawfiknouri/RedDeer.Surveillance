using System;
using Domain.Equity.Streams.Interfaces;
using Surveillance.Rule_Parameters.Filter;
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
