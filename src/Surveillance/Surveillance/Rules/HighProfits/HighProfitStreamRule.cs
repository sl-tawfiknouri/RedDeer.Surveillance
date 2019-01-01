using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly IHighProfitsRuleParameters _parameters;
        protected readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        protected readonly IUniverseAlertStream _alertStream;
        private readonly bool _submitRuleBreaches;

        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly IUniverseOrderFilter _orderFilter;

        protected bool MarketClosureRule = false;

        public HighProfitStreamRule(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            bool submitRuleBreaches,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromHours(8),
                DomainV2.Scheduling.Rules.HighProfits,
                HighProfitRuleFactory.Version,
                "High Profit Rule",
                ruleCtx,
                factory,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _submitRuleBreaches = submitRuleBreaches;
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator 
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual bool RunRuleGuard(ITradingHistoryStack history)
        {
            return true;
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            if (!RunRuleGuard(history))
            {
                return;
            }

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at => at.OrderStatus() == OrderStatus.Filled)
                .ToList();

            if (!liveTrades.Any())
            {
                return;
            }

            var targetCurrency = new DomainV2.Financial.Currency(_parameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency =
                liveTrades.Any()
                 && (liveTrades.All(x =>
                     string.Equals(
                         x.OrderCurrency.Value,
                         targetCurrency.Value,
                         StringComparison.InvariantCultureIgnoreCase)));

            var costCalculator = GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, UniverseDateTime, _ruleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(liveTrades, UniverseDateTime, _ruleCtx, UniverseMarketCache);

            costTask.Wait();
            revenueTask.Wait();

            var cost = costTask.Result;
            var revenue = revenueTask.Result;

            if (revenue == null)
            {
                return;
            }

            if (cost == null)
            {
                _logger.LogError("High profits rule - something went horribly wrong. We have calculable revenues but not costs");
                return;
            }

            if (revenue.Value.Value <= 0
                || cost.Value.Value <= 0)
            {
                _logger.LogInformation($"High profit rules had revenue of {revenue.Value.Value} and cost of {cost.Value.Value}. Returning.");
                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = (revenue.Value.Value / cost.Value.Value) - 1;

            var hasHighProfitAbsolute = HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (_parameters.UseCurrencyConversions
                && !string.IsNullOrEmpty(_parameters.HighProfitCurrencyConversionTargetCurrency))
            {
                exchangeRateProfits = SetExchangeRateProfits(liveTrades);
            }

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                WriteAlertToMessageSender(
                    activeTrades,
                    absoluteProfit.Value,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits);
            }
        }

        private IExchangeRateProfitBreakdown SetExchangeRateProfits(List<Order> liveTrades)
        {
            var currency = new DomainV2.Financial.Currency(_parameters.HighProfitCurrencyConversionTargetCurrency);
            var buys = new TradePosition(liveTrades.Where(lt =>
                lt.OrderPosition == OrderPositions.BUY 
                || lt.OrderPosition == OrderPositions.SHORT).ToList());

            var sells = new TradePosition(liveTrades.Where(lt =>
                lt.OrderPosition == OrderPositions.SELL
                || lt.OrderPosition == OrderPositions.COVER).ToList());

            var exchangeRateProfitsTask =
                _exchangeRateProfitCalculator.ExchangeRateMovement(
                    buys,
                    sells,
                    currency,
                    _ruleCtx);

            exchangeRateProfitsTask.Wait();
            var exchangeRateProfits = exchangeRateProfitsTask.Result;

            return exchangeRateProfits;
        }

        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, DomainV2.Financial.Currency targetCurrency)
        {
            if (!_parameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                return _costCalculatorFactory.CostCalculator();
            }

            return _costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, DomainV2.Financial.Currency targetCurrency)
        {
            if (!_parameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                var calculator =
                    MarketClosureRule
                        ? _revenueCalculatorFactory.RevenueCalculatorMarkingTheClose()
                        : _revenueCalculatorFactory.RevenueCalculator();

                return calculator;
            }
           
            var currencyConvertingCalculator =
                MarketClosureRule
                    ? _revenueCalculatorFactory.RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                    : _revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(targetCurrency);

            return currencyConvertingCalculator;
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private void WriteAlertToMessageSender(
            Stack<Order> activeTrades,
            decimal absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage,
            IExchangeRateProfitBreakdown breakdown)
        {
            var security = activeTrades.FirstOrDefault(at => at?.Instrument != null)?.Instrument;

            _logger.LogInformation($"High Profits Rule breach detected for {security?.Identifiers}. Writing breach to message sender.");

            var position = new TradePosition(activeTrades.ToList());
            var breach =
                new HighProfitRuleBreach(
                    _parameters,
                    absoluteProfit,
                    _parameters.HighProfitCurrencyConversionTargetCurrency,
                    profitRatio,
                    security,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    position,
                    MarketClosureRule,
                    breakdown);

            var alertEvent = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return _parameters.HighProfitPercentageThreshold.HasValue
               && _parameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        private bool HasHighProfitAbsolute(CurrencyAmount absoluteProfits)
        {
            if (_parameters.HighProfitAbsoluteThreshold == null)
            {
                return false;
            }

            if (_parameters.UseCurrencyConversions
                && !string.Equals(
                _parameters.HighProfitCurrencyConversionTargetCurrency,
                absoluteProfits.Currency.Value,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _ruleCtx.EventException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
                throw new InvalidOperationException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= _parameters.HighProfitAbsoluteThreshold;
        }    
        
        protected override void Genesis()
        {
            _logger.LogInformation("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading Opened for exchange {exchange.MarketId} in the High Profit Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading closed for exchange {exchange.MarketId} in the High Profit Rule. Running market closure virtual profits check.");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Universe Eschaton occurred in the High Profit Rule");

            if (!MarketClosureRule)
            {
                RunRuleForAllTradingHistories();
            }

            if (_submitRuleBreaches)
            {
                var alert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, null, _ruleCtx, true);
                _alertStream.Add(alert);
            }

            _ruleCtx?.EndEvent();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
