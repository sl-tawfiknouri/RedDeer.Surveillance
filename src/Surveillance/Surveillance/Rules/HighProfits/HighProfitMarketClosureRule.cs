﻿using System.Linq;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitMarketClosureRule : HighProfitStreamRule, IHighProfitMarketClosureRule
    {
        public HighProfitMarketClosureRule(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            ILogger<HighProfitsRule> logger)
            : base(
                parameters, 
                ruleCtx,
                alertStream,
                true,
                costCalculatorFactory,
                revenueCalculatorFactory,
                exchangeRateProfitCalculator,
                logger)
        {
            MarketClosureRule = true;
        }

        protected override bool RunRuleGuard(ITradingHistoryStack history)
        {
            var activeWindow = history.ActiveTradeHistory();

            if (!activeWindow.Any())
            {
                return false;
            }

            var tradeBuy =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => aw.OrderPosition == OrderPositions.BUY
                                 || aw.OrderPosition == OrderPositions.SHORT)
                    .ToList();

            var tradeSell =
                activeWindow
                    .Where(aw => aw != null)
                    .Where(aw => 
                        aw.OrderPosition == OrderPositions.SELL
                        || aw.OrderPosition == OrderPositions.COVER)
                    .ToList();

            var securitiesBrought = tradeBuy.Sum(tb => tb.OrderFilledVolume);
            var securitiesSold = tradeSell.Sum(tb => tb.OrderFilledVolume);

            if (securitiesBrought > securitiesSold)
            {
                return true;
            }

            var position = new TradePosition(activeWindow.ToList());

            var message = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, position, _ruleCtx) { IsRemoveEvent = true };
            _alertStream.Add(message);

            return false;
        }
    }
}
