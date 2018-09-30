using System;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.High_Profits;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

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

        public IHighProfitRule Build(IHighProfitsRuleParameters parameters)
        {
            return new HighProfitsRule(_currencyConverter, _messageSender, parameters, _logger);
        }
    }
}