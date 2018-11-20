using System;
using Domain.Equity.Streams.Interfaces;
using Surveillance.Analytics.Streams.Factory.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;

namespace Surveillance.Analytics.Streams.Factory
{
    public class UniverseAlertStreamFactory : IUniverseAlertStreamFactory
    {
        private readonly IUnsubscriberFactory<IUniverseAlertEvent> _unsubscriberFactory;

        public UniverseAlertStreamFactory(IUnsubscriberFactory<IUniverseAlertEvent> unsubscriberFactory)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public IUniverseAlertStream Build()
        {
            return new UniverseAlertStream(_unsubscriberFactory);
        }
    }
}
