using System;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IHighProfitRuleCachedMessageSender _messageSender;
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitRuleFactory(
            ICurrencyConverter currencyConverter,
            IHighProfitRuleCachedMessageSender messageSender,
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ILogger<HighProfitsRule> logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket)
        {
            var stream = new HighProfitStreamRule(_currencyConverter, _messageSender, parameters, ruleCtxStream, false, _logger);
            var marketClosure = new HighProfitMarketClosureRule(_currencyConverter, _messageSender, parameters, ruleCtxMarket, _logger);
            var multiverseTransformer = new MarketCloseMultiverseTransformer(_unsubscriberFactory);
            multiverseTransformer.Subscribe(marketClosure);

            return new HighProfitsRule(stream, multiverseTransformer);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}