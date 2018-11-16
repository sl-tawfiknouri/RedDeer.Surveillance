using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.WashTrade
{
    /// <summary>
    /// This trade rule is geared towards catching alpha wash trade breaches
    /// These are attempts to manipulate the market at large through publicly observable information
    /// By making a large amount of trades in an otherwise unremarkable stock the objective is to increase
    /// interest in trading the stock which will open up opportunities to profit by the wash traders
    /// </summary>
    public class WashTradeRule : BaseUniverseRule, IWashTradeRule
    {
        private int _alerts;
        private readonly ILogger _logger;
        private readonly IWashTradeRuleParameters _parameters;

        public WashTradeRule(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ILogger logger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromDays(1),
                Domain.Scheduling.Rules.WashTrade,
                Versioner.Version(1, 0),
                "Wash Trade Rule",
                ruleCtx,
                logger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            // ok so grab all the trades for the window
            // then we want to begin pairing up wash trading
            // what we're specifically looking for are buys/sells to be paired up around the same prices
            // also a minimum amount of trades in the wash trade process is also needed

            // I don't think we can expect a specific directionality on trades in any subset of time

            // i.e. buy buy sell buy sell buy buy sell sell sel sel sel sel buy is OK
            // what we need to do is begin pairing up trades in opposite directions by their "best fit" partner i.e. if buy/sell on both sides we would still expect there to be accounting matching

            // now attempts to disguise this can be done by buy at P1, sell at P2, vary volume to make up for P1; P2
            // so we're looking for trading the same [VALUE] of the stocks

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades =
                activeTrades
                    .Where(at =>
                        at.OrderStatus == OrderStatus.Fulfilled
                        || at.OrderStatus == OrderStatus.PartialFulfilled)
                    .ToList();

            var averagePositionCheck = ValueOfPositionChange(liveTrades);
            var pairingPositionsCheck = PairingBuySells(liveTrades);

            if (averagePositionCheck == null
                && pairingPositionsCheck == null)
            {
                return;
            }

            _alerts += 1;
            _logger.LogInformation($"Wash Trade Rule incrementing alert count to {_alerts}");


            // push it onto the message sender

        }

        public WashTradeRuleBreach ValueOfPositionChange(List<TradeOrderFrame> activeTrades)
        {
            if (activeTrades == null 
                || !activeTrades.Any())
            {
                return null;
            }
        
            if (activeTrades.Count < _parameters.AveragePositionMinimumNumberOfTrades)
            {
                return null;
            }

            var buyPosition = new List<TradeOrderFrame>(activeTrades.Where(at => at.Position == OrderPosition.Buy).ToList());
            var sellPosition = new List<TradeOrderFrame>(activeTrades.Where(at => at.Position == OrderPosition.Sell).ToList());

            var valueOfBuy = buyPosition.Sum(bp => bp.FulfilledVolume * (bp.ExecutedPrice?.Value ?? 0));
            var valueOfSell = sellPosition.Sum(sp => sp.FulfilledVolume * (sp.ExecutedPrice?.Value ?? 0));

            if (valueOfBuy == 0)
            {
                return null;
            }

            if (valueOfSell == 0)
            {
                return null;
            }

            var relativeValue = Math.Abs((valueOfBuy / valueOfSell) - 1);

            if (relativeValue > _parameters.AveragePositionMaximumAbsoluteValueChangeAmount.GetValueOrDefault(0))
            {
                return null;
            }

            if (_parameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
            {
                _logger.LogInformation("WashTradeRule found an average position breach and does not have an absolute limit set. Returning with average position breach");
                var activeTradePosition = new TradePosition(activeTrades);

                return new WashTradeRuleBreach(_parameters, activeTradePosition, true, activeTrades.Count, relativeValue, null);
            }

            // TODO do the absolute value check - will involve x-change stuff

            return null;
        }

        public WashTradeRuleBreach PairingBuySells(List<TradeOrderFrame> activeTrades)
        {
            // percentage of trades that are 'paired up' i.e.
            // high trading activity without taking a position
            // in the equity

            return null;
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occured in the Wash Trade Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation($"Eschaton occured in the Wash Trade Rule");

            RuleCtx?.UpdateAlertEvent(_alerts);
            RuleCtx?.EndEvent();
            _alerts = 0;
        }
    }
}
