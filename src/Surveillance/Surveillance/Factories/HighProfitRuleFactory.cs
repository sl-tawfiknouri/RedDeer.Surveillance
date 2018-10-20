using System;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighProfits;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IHighProfitRuleCachedMessageSender _messageSender;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitRuleFactory(
            ICurrencyConverter currencyConverter,
            IHighProfitRuleCachedMessageSender messageSender,
            ILogger<HighProfitsRule> logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighProfitRule Build(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket)
        {
            var stream = new HighProfitStreamRule(_currencyConverter, _messageSender, parameters, ruleCtxStream, _logger);
            var marketClosure = new HighProfitMarketClosureRule(_currencyConverter, _messageSender, parameters, ruleCtxMarket, _logger);

            return new HighProfitsRule(stream, marketClosure);
        }

        public string RuleVersion => Versioner.Version(1, 0);
    }
}