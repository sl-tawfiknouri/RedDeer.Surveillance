using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.High_Profits;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Factories
{
    public class HighProfitRuleFactory : IHighProfitRuleFactory
    {
        private readonly IExchangeRateApiCachingDecoratorRepository _apiRepository;
        private readonly IHighProfitRuleCachedMessageSender _messageSender;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitRuleFactory(
            IExchangeRateApiCachingDecoratorRepository apiRepository,
            IHighProfitRuleCachedMessageSender messageSender,
            ILogger<HighProfitsRule> logger)
        {
            _apiRepository = apiRepository ?? throw new ArgumentNullException(nameof(apiRepository));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHighProfitRule Build(IHighProfitsRuleParameters parameters)
        {
            return new HighProfitsRule(_apiRepository, _messageSender, parameters, _logger);
        }
    }
}