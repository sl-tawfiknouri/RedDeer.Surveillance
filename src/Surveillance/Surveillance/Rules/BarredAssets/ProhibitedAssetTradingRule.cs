using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.BarredAssets;
using System;

namespace Surveillance.Rules.ProhibitedAssetTradingRule
{
    /// <summary>
    /// Trading can be prohibited against certain assets
    /// This may apply to all employees; groups of employees; or a singular employee
    /// </summary>
    public class ProhibitedAssetTradingRule : IProhibitedAssetTradingRule
    {
        private ILogger _logger;
        private IProhibitedAssetsRepository _assetsRepository;

        public ProhibitedAssetTradingRule(
            ILogger<ProhibitedAssetTradingRule> logger,
            IProhibitedAssetsRepository assetsRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assetsRepository = assetsRepository ?? throw new ArgumentNullException(nameof(assetsRepository));
        }

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
                _logger.LogError($"ILLEGAL TRADE DETECTED: PROHIBITED ASSET {value?.Security?.Name}");
            }
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
