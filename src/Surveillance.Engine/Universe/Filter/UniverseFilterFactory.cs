namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    public class UniverseFilterFactory : IUniverseFilterFactory
    {
        private readonly IHighMarketCapFilterFactory _highMarketCapFilterFactory;

        private readonly ILoggerFactory _loggerFactory;

        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;

        public UniverseFilterFactory(
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            IHighMarketCapFilterFactory highMarketCapFilterFactory,
            ILoggerFactory loggerFactory)
        {
            this._unsubscriberFactory =
                unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            this._highMarketCapFilterFactory = highMarketCapFilterFactory
                                               ?? throw new ArgumentNullException(nameof(highMarketCapFilterFactory));
            this._loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IUniverseFilterService Build(
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            DecimalRangeRuleFilter marketCap,
            RuleRunMode ruleRunMode,
            string ruleName,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext operationRunRuleContext)
        {
            var highMarketCapFilter = this._highMarketCapFilterFactory.Build(
                ruleRunMode,
                marketCap,
                ruleName,
                universeDataRequestsSubscriber,
                operationRunRuleContext);

            return new UniverseFilterService(
                this._unsubscriberFactory,
                highMarketCapFilter,
                accounts,
                traders,
                markets,
                funds,
                strategies,
                sectors,
                industries,
                regions,
                countries,
                this._loggerFactory.CreateLogger<UniverseFilterService>());
        }
    }
}