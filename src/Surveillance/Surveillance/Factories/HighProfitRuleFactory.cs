using System;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly IUnsubscriberFactory<IUniverseEvent> _unsubscriberFactory;
        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitRuleFactory(
            IUnsubscriberFactory<IUniverseEvent> unsubscriberFactory,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            ILogger<HighProfitsRule> logger)
        {
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket,
            IUniverseAlertStream alertStream)
        {
            var stream = new HighProfitStreamRule(
                parameters,
                ruleCtxStream,
                alertStream,
                false,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _logger);

            var marketClosure = new HighProfitMarketClosureRule(
                parameters,
                ruleCtxMarket,
                alertStream,
                _costCalculatorFactory,
                _revenueCalculatorFactory,
                _exchangeRateProfitCalculator,
                _logger);

            var multiverseTransformer = new MarketCloseMultiverseTransformer(_unsubscriberFactory);
            multiverseTransformer.Subscribe(marketClosure);

            return new HighProfitsRule(stream, multiverseTransformer);
        }

        public static string Version => Versioner.Version(2, 0);
    }
}