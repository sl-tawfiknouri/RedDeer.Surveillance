using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Finance;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
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
        private readonly IWashTradeCachedMessageSender _messageSender;
        private readonly ICurrencyConverter _currencyConverter;

        public WashTradeRule(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IWashTradeCachedMessageSender messageSender,
            ICurrencyConverter currencyConverter,
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
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var activeTrades = history.ActiveTradeHistory();

            var liveTrades =
                activeTrades
                    .Where(at =>
                        at.OrderStatus == OrderStatus.Fulfilled
                        || at.OrderStatus == OrderStatus.PartialFulfilled)
                    .ToList();

            if (!liveTrades?.Any() ?? true)
            {
                return;
            }

            var averagePositionCheckTask = ValueOfPositionChange(liveTrades);
            averagePositionCheckTask.Wait();
            var averagePositionCheck = averagePositionCheckTask.Result;

            var pairingPositionsCheck = PairingBuySells(liveTrades);

            if ((averagePositionCheck == null || !averagePositionCheck.AveragePositionRuleBreach)
                && pairingPositionsCheck == null)
            {
                return;
            }

            _alerts += 1;
            _logger.LogInformation($"Wash Trade Rule incrementing alert count to {_alerts}");

            var trades = new TradePosition(liveTrades);
            var security = liveTrades?.FirstOrDefault()?.Security;

            var breach = new WashTradeRuleBreach(_parameters, trades, security, averagePositionCheck);
            _messageSender?.Send(breach);
        }

        public async Task<WashTradeRuleBreach.WashTradeAveragePositionBreach> ValueOfPositionChange(List<TradeOrderFrame> activeTrades)
        {
            if (activeTrades == null 
                || !activeTrades.Any())
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }
        
            if (activeTrades.Count < _parameters.AveragePositionMinimumNumberOfTrades)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            var buyPosition = new List<TradeOrderFrame>(activeTrades.Where(at => at.Position == OrderPosition.Buy).ToList());
            var sellPosition = new List<TradeOrderFrame>(activeTrades.Where(at => at.Position == OrderPosition.Sell).ToList());

            var valueOfBuy = buyPosition.Sum(bp => bp.FulfilledVolume * (bp.ExecutedPrice?.Value ?? 0));
            var valueOfSell = sellPosition.Sum(sp => sp.FulfilledVolume * (sp.ExecutedPrice?.Value ?? 0));

            if (valueOfBuy == 0)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (valueOfSell == 0)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            var relativeValue = Math.Abs((valueOfBuy / valueOfSell) - 1);

            if (relativeValue > _parameters.AveragePositionMaximumAbsoluteValueChangeAmount.GetValueOrDefault(0))
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            if (_parameters.AveragePositionMaximumAbsoluteValueChangeAmount == null
                || string.IsNullOrWhiteSpace(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency))
            {
                _logger.LogInformation("WashTradeRule found an average position breach and does not have an absolute limit set. Returning with average position breach");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                    (true,
                    activeTrades.Count,
                    relativeValue,
                    null);
            }

            var absDifference = Math.Abs(valueOfBuy - valueOfSell);
            var currency = new Domain.Finance.Currency(activeTrades.FirstOrDefault()?.OrderCurrency);
            var absCurrencyAmount = new CurrencyAmount(absDifference, currency);

            var targetCurrency = new Domain.Finance.Currency(_parameters.AveragePositionMaximumAbsoluteValueChangeCurrency);
            var convertedCurrency = await _currencyConverter.Convert(new[] {absCurrencyAmount}, targetCurrency, UniverseDateTime, RuleCtx);

            if (convertedCurrency == null)
            {
                _logger.LogError($"WashTradeRule was not able to determine currency conversion - preferring to raise alert in lieu of necessary exchange rate information");

                return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                    (true,
                     activeTrades.Count,
                     relativeValue,
                    null);
            }

            if (_parameters.AveragePositionMaximumAbsoluteValueChangeAmount < convertedCurrency.Value.Value)
            {
                return WashTradeRuleBreach.WashTradeAveragePositionBreach.None();
            }

            return new WashTradeRuleBreach.WashTradeAveragePositionBreach
                (true,
                activeTrades.Count,
                relativeValue,
                convertedCurrency);
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
            _messageSender?.Flush(RuleCtx);
            RuleCtx?.EndEvent();

            _alerts = 0;
        }
    }
}
