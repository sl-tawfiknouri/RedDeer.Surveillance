using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.WashTrade;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories
{
    public class WashTradeRuleFactory : IWashTradeRuleFactory
    {
        private readonly ILogger _logger;

        public string RuleVersion { get; } = Versioner.Version(1, 0);

        public WashTradeRuleFactory(ILogger<WashTradeRule> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IWashTradeRule Build(IWashTradeRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx)
        {
            if (ruleCtx == null)
            {
                throw new ArgumentNullException(nameof(ruleCtx));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return new WashTradeRule(parameters, ruleCtx, _logger);
        }
    }
}
