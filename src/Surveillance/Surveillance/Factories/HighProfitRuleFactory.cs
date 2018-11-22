using System;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly IHighProfitRuleCachedMessageSender _messageSender;
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitRuleFactory(
            IHighProfitRuleCachedMessageSender messageSender,
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            ILogger<HighProfitsRule> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket)
        {
            var stream = new HighProfitStreamRule(
                _messageSender,
                parameters,
                ruleCtxStream,
                false,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _logger);

            var marketClosure = new HighProfitMarketClosureRule(
                _messageSender,
                parameters,
                ruleCtxMarket,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _logger);

            var multiverseTransformer = new MarketCloseMultiverseTransformer(_unsubscriberFactory);
            multiverseTransformer.Subscribe(marketClosure);

            return new HighProfitsRule(stream, multiverseTransformer);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}