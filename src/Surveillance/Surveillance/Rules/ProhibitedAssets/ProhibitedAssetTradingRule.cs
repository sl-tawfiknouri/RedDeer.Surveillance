using System;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.ElasticSearchDtos.Rules;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.ProhibitedAssets.Interfaces;

namespace Surveillance.Rules.ProhibitedAssets
{
    /// <summary>
    /// Trading can be prohibited against certain assets
    /// This may apply to all employees; groups of employees; or a singular employee
    /// This is a test rule - I'm using it a a base to test logic before implementing other rules.
    /// </summary>
    public class ProhibitedAssetTradingRule : IProhibitedAssetTradingRule
    {
        private readonly ILogger _logger;
        private readonly IProhibitedAssetsRepository _assetsRepository;
        private readonly IRuleBreachFactory _ruleBreachFactory;
        private readonly IRuleBreachRepository _ruleBreachRepository;

        public ProhibitedAssetTradingRule(
            ILogger<ProhibitedAssetTradingRule> logger,
            IProhibitedAssetsRepository assetsRepository,
            IRuleBreachFactory ruleBreachFactory,
            IRuleBreachRepository ruleBreachRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assetsRepository = assetsRepository ?? throw new ArgumentNullException(nameof(assetsRepository));
            _ruleBreachFactory = ruleBreachFactory ?? throw new ArgumentNullException(nameof(ruleBreachFactory));
            _ruleBreachRepository = ruleBreachRepository ?? throw new ArgumentNullException(nameof(ruleBreachRepository));
        }

        public Rules Rule { get; } = Rules.ProhibitedAssets;
        public string Version { get; } = "V1.0";

        public void OnCompleted()
        {
            _logger.LogInformation("Prohibited asset trading rule reached stream completion");
        }

        public void OnError(Exception error)
        {
            _logger.LogError("Prohibited asset trading rule observed an error in its subscription", error);
        }

        public void OnNext(TradeOrderFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (TradeOrderAgainstProhibitedAsset(value))
            {
                RuleBreached(value);
            }
        }

        private void RuleBreached(TradeOrderFrame value)
        {
            _logger.LogError($"ILLEGAL TRADE DETECTED: PROHIBITED ASSET {value?.Security?.Name}");

            var timeBreachDetected = DateTime.UtcNow;
            var description = $"The prohibited asset trading rule detected a breach. The prohibited security that was traded was {value?.Security?.Name}. Full details {value}";

            var prohibitedAssetBreachDocument = _ruleBreachFactory.Build(
                RuleBreachCategories.ProhibitedAsset,
                timeBreachDetected,
                timeBreachDetected,
                description);

            _ruleBreachRepository.Save(prohibitedAssetBreachDocument);
        }

        /// <summary>
        /// Checks the traded security name against a list of prohibited equities
        /// </summary>
        /// <returns>boolean true/false true if the trade is prohibited</returns>
        private bool TradeOrderAgainstProhibitedAsset(TradeOrderFrame frame)
        {
            return _assetsRepository.ProhibitedEquities?.Contains(frame?.Security?.Name) ?? false;
        }
    }
}
