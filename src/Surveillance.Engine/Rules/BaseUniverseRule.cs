namespace Surveillance.Engine.Rules.Rules
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    /// <summary>
    /// The base universe rule.
    /// </summary>
    public abstract class BaseUniverseRule : IUniverseRule
    {
        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The trade backward window size.
        /// </summary>
        protected readonly TimeSpan TradeBackwardWindowSize;

        /// <summary>
        /// The forward window size.
        /// </summary>
        protected readonly TimeSpan ForwardWindowSize;

        /// <summary>
        /// The universe equity intraday cache.
        /// </summary>
        protected IUniverseEquityIntraDayCache UniverseEquityIntradayCache;

        /// <summary>
        /// The universe equity interday cache.
        /// </summary>
        protected IUniverseEquityInterDayCache UniverseEquityInterdayCache;

        /// <summary>
        /// The universe fixed income intraday cache.
        /// </summary>
        protected IUniverseFixedIncomeIntraDayCache UniverseFixedIncomeIntradayCache;

        /// <summary>
        /// The universe fixed income interday cache.
        /// </summary>
        protected IUniverseFixedIncomeInterDayCache UniverseFixedIncomeInterdayCache;

        /// <summary>
        /// The universe event.
        /// </summary>
        protected IUniverseEvent UniverseEvent;

        /// <summary>
        /// These are paid up with the delayed trading histories to create the illusion of future data analysis
        /// whilst maintaining a singular set of abstractions around the backward aspect of analysis
        /// </summary>
        protected IUniverseEquityIntraDayCache FutureUniverseEquityIntradayCache;

        /// <summary>
        /// These are paid up with the delayed trading histories to create the illusion of future data analysis
        /// whilst maintaining a singular set of abstractions around the backward aspect of analysis
        /// </summary>
        protected IUniverseFixedIncomeIntraDayCache FutureUniverseFixedIncomeIntradayCache;

        /// <summary>
        /// The trading history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingHistory;

        /// <summary>
        /// The trading fills history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingFillsHistory;

        /// <summary>
        /// The trading initial history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingInitialHistory;

        /// <summary>
        /// The delayed trading history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingHistory;

        /// <summary>
        /// The delayed trading fills history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingFillsHistory;

        /// <summary>
        /// The delayed trading initial history.
        /// </summary>
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingInitialHistory;

        /// <summary>
        /// The schedule.
        /// </summary>
        protected ScheduledExecution Schedule;

        /// <summary>
        /// The universe date time.
        /// </summary>
        protected DateTime UniverseDateTime;

        /// <summary>
        /// The has reached end of universe.
        /// </summary>
        protected bool HasReachedEndOfUniverse;

        /// <summary>
        /// The has reached future universe epoch.
        /// </summary>
        protected bool HasReachedFutureUniverseEpoch;

        /// <summary>
        /// The rule ctx.
        /// </summary>
        protected readonly ISystemProcessOperationRunRuleContext RuleCtx;

        /// <summary>
        /// The run mode.
        /// </summary>
        protected readonly RuleRunMode RunMode;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The trading stack logger.
        /// </summary>
        private readonly ILogger<TradingHistoryStack> tradingStackLogger;

        /// <summary>
        /// The lock.
        /// </summary>
        private readonly object @lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUniverseRule"/> class.
        /// </summary>
        /// <param name="tradeBackwardWindowSize">
        /// The trade backward window size.
        /// </param>
        /// <param name="marketBackwardWindowSize">
        /// The market backward window size.
        /// </param>
        /// <param name="forwardWindowSize">
        /// The forward window size.
        /// </param>
        /// <param name="rules">
        /// The rules.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="equityFactory">
        /// The equity factory.
        /// </param>
        /// <param name="fixedIncomeFactory">
        /// The fixed income factory.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        protected BaseUniverseRule(
            TimeSpan tradeBackwardWindowSize,
            TimeSpan marketBackwardWindowSize,
            TimeSpan forwardWindowSize,
            Rules rules,
            string version,
            string name,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseEquityMarketCacheFactory equityFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeFactory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
        {
            this.TradeBackwardWindowSize = tradeBackwardWindowSize;
            this.ForwardWindowSize = forwardWindowSize;

            this.Rule = rules;
            this.Version = version ?? string.Empty;

            this.UniverseEquityIntradayCache =
                equityFactory?.BuildIntraday(marketBackwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(equityFactory));

            this.FutureUniverseEquityIntradayCache =
                equityFactory?.BuildIntraday(forwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(equityFactory));

            this.UniverseEquityInterdayCache =
                equityFactory?.BuildInterday(runMode)
                ?? throw new ArgumentNullException(nameof(equityFactory));

            this.UniverseFixedIncomeIntradayCache =
                fixedIncomeFactory?.BuildIntraday(marketBackwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));

            this.FutureUniverseFixedIncomeIntradayCache =
                fixedIncomeFactory?.BuildIntraday(forwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));

            this.UniverseFixedIncomeInterdayCache =
                fixedIncomeFactory?.BuildInterday(runMode)
                ?? throw new ArgumentNullException(nameof(fixedIncomeFactory));

            this.TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.TradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            this.DelayedTradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.DelayedTradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.DelayedTradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            this.RuleCtx = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
            this.name = name ?? "Unnamed rule";
            this.RunMode = runMode;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tradingStackLogger = tradingStackLogger ?? throw new ArgumentNullException(nameof(tradingStackLogger));
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        public Rules Rule { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.logger?.LogInformation($"Universe Rule {this.name} completed its universe stream");
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.logger?.LogError($"{this.name} {Version} {error}");
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            var filteredValue = this.Filter(value);
            if (filteredValue == null)
            {
                this.logger?.LogInformation($"base universe event at {value.EventTime} filtered out. Skipping event.");
                return;
            }

            this.UniverseEvent = value;
            this.logger?.LogTrace($"{value} universe event passed to {this.name} at universe time {value.EventTime}.");

            lock (this.@lock)
            {
                switch (value.StateChange)
                {
                    case UniverseStateEvent.Genesis:
                        this.Genesis(value);
                        break;
                    case UniverseStateEvent.EquityIntraDayTick:
                        this.EquityIntraDay(value);
                        this.FutureEquityIntraDay(value);
                        break;
                    case UniverseStateEvent.EquityInterDayTick:
                        this.EquityInterDay(value);
                        break;
                    case UniverseStateEvent.FixedIncomeIntraDayTick:
                        this.FixedIncomeIntraDay(value);
                        this.FutureFixedIncomeIntraDay(value);
                        break;
                    case UniverseStateEvent.FixedIncomeInterDayTick:
                        this.FixedIncomeInterDay(value);
                        break;
                    case UniverseStateEvent.OrderPlaced:
                        this.TradeSubmitted(value);
                        this.TradeSubmittedDelay(value);
                        break;
                    case UniverseStateEvent.Order:
                        this.Trade(value);
                        this.TradeDelay(value);
                        break;
                    case UniverseStateEvent.OrderFilled:
                        this.TradeFilled(value);
                        this.TradeFilledDelay(value);
                        break;
                    case UniverseStateEvent.ExchangeOpen:
                        this.MarketOpened(value);
                        break;
                    case UniverseStateEvent.ExchangeClose:
                        this.MarketClosed(value);
                        break;
                    case UniverseStateEvent.Eschaton:
                        this.Eschaton(value);
                        break;
                    case UniverseStateEvent.Unknown:
                        this.logger?.LogWarning($"Universe rule {this.name} received an unknown event");
                        this.RuleCtx.EventException($"Universe rule {this.name} received an unknown event");
                        break;
                    case UniverseStateEvent.EpochFutureUniverse:
                        this.HasReachedFutureUniverseEpoch = true;
                        break;
                }
            }
        }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public abstract IRuleDataConstraint DataConstraints();

        /// <summary>
        /// The base clone.
        /// </summary>
        public void BaseClone()
        {
            this.UniverseEquityIntradayCache = (IUniverseEquityIntraDayCache)this.UniverseEquityIntradayCache.Clone();
            this.UniverseEquityInterdayCache = (IUniverseEquityInterDayCache)this.UniverseEquityInterdayCache.Clone();
            this.FutureUniverseEquityIntradayCache = (IUniverseEquityIntraDayCache)this.FutureUniverseEquityIntradayCache.Clone();
            this.UniverseFixedIncomeInterdayCache = (IUniverseFixedIncomeInterDayCache)this.UniverseFixedIncomeInterdayCache.Clone();
            this.UniverseFixedIncomeIntradayCache = (IUniverseFixedIncomeIntraDayCache)this.UniverseFixedIncomeIntradayCache.Clone();
            this.FutureUniverseFixedIncomeIntradayCache = (IUniverseFixedIncomeIntraDayCache)this.FutureUniverseFixedIncomeIntradayCache.Clone();
            this.TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingHistory);
            this.TradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingFillsHistory);
            this.TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingInitialHistory);
            this.DelayedTradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.DelayedTradingHistory);
            this.DelayedTradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.DelayedTradingFillsHistory);
            this.DelayedTradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.DelayedTradingInitialHistory);
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected abstract IUniverseEvent Filter(IUniverseEvent value);

        /// <summary>
        /// The genesis.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void Genesis(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is ScheduledExecution value))
            {
                return;
            }

            this.logger?.LogInformation($"Genesis event in base universe rule occuring for rule {this.name} | event/universe time {universeEvent.EventTime} | correlation id {value.CorrelationId} | time series initiation  {value.TimeSeriesInitiation} | time series termination {value.TimeSeriesTermination}");

            this.Schedule = value;
            this.UniverseDateTime = universeEvent.EventTime;
            this.Genesis();
        }

        /// <summary>
        /// The equity intra day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void EquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Equity intra day event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseEquityIntradayCache.Add(value);
        }

        /// <summary>
        /// The future equity intra day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void FutureEquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Equity intra day event (future) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.FutureUniverseEquityIntradayCache.Add(value);
        }

        /// <summary>
        /// The equity inter day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Equity inter day event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseEquityInterdayCache.Add(value);
        }

        /// <summary>
        /// The fixed income intra day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void FixedIncomeIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is FixedIncomeIntraDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Fixed income intra day event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseFixedIncomeIntradayCache.Add(value);
        }

        /// <summary>
        /// The future fixed income intra day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void FutureFixedIncomeIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is FixedIncomeIntraDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Fixed income intra day event (future) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.FutureUniverseFixedIncomeIntradayCache.Add(value);
        }

        /// <summary>
        /// The fixed income inter day.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void FixedIncomeInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is FixedIncomeInterDayTimeBarCollection value))
            {
                return;
            }

            this.logger?.LogInformation($"Fixed income inter day event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseFixedIncomeInterdayCache.Add(value);
        }

        /// <summary>
        /// The trade submitted.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void TradeSubmitted(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (this.HasReachedFutureUniverseEpoch)
            {
                return;
            }

            this.logger?.LogTrace($"Trade placed event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                this.UpdateTradeSubmittedTradingHistories(
                    value,
                    this.TradingInitialHistory,
                    this.TradeBackwardWindowSize,
                    null);

            this.RunInitialSubmissionEvent(updatedHistory);
        }

        /// <summary>
        /// The trade submitted delay.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void TradeSubmittedDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            this.logger?.LogTrace($"Trade placed event (delay) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                this.UpdateTradeSubmittedTradingHistories(
                    value,
                    this.DelayedTradingInitialHistory,
                    this.TradeBackwardWindowSize,
                    this.ForwardWindowSize);

            this.RunInitialSubmissionEventDelayed(updatedHistory);
        }

        /// <summary>
        /// The update trade submitted trading histories.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradingHistory">
        /// The trading history.
        /// </param>
        /// <param name="backwardWindowSize">
        /// The backward window size.
        /// </param>
        /// <param name="forwardWindowSize">
        /// The forward window size.
        /// </param>
        /// <returns>
        /// The <see cref="ITradingHistoryStack"/>.
        /// </returns>
        private ITradingHistoryStack UpdateTradeSubmittedTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingHistory,
            TimeSpan backwardWindowSize,
            TimeSpan? forwardWindowSize)
        {
            if (!tradingHistory.ContainsKey(order.Instrument.Identifiers))
            {
                ITradingHistoryStack history =
                    new TradingHistoryStack(
                        backwardWindowSize, 
                        i => i.PlacedDate.GetValueOrDefault(), 
                        this.tradingStackLogger);

                ITradingHistoryStack historyDecorator = 
                    forwardWindowSize != null 
                        ? new TradingHistoryDelayedDecorator(history, forwardWindowSize.GetValueOrDefault())
                        : null;

                var stack = historyDecorator ?? history;
                stack.Add(order, order.PlacedDate.GetValueOrDefault());
                tradingHistory.TryAdd(order.Instrument.Identifiers, stack);
            }
            else
            {
                tradingHistory.TryGetValue(order.Instrument.Identifiers, out var history);

                history?.Add(order, order.PlacedDate.GetValueOrDefault());
                history?.ArchiveExpiredActiveItems(order.PlacedDate.GetValueOrDefault());
            }

            tradingHistory.TryGetValue(order.Instrument.Identifiers, out var updatedHistory);

            return updatedHistory;
        }

        /// <summary>
        /// The trade.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void Trade(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (this.HasReachedFutureUniverseEpoch)
            {
                return;
            }

            this.logger?.LogTrace($"Trade event (status changed) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = 
                this.UpdateTradeLatestTradingHistories(
                    value, 
                    this.TradingHistory, 
                    this.TradeBackwardWindowSize, 
                    null);

            this.RunPostOrderEvent(updatedHistory);
        }

        /// <summary>
        /// The trade delay.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void TradeDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            this.logger?.LogTrace($"Trade event (status changed delayed) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = 
                this.UpdateTradeLatestTradingHistories(
                    value, 
                    this.DelayedTradingHistory,
                    this.TradeBackwardWindowSize, 
                    this.ForwardWindowSize);

            this.RunPostOrderEventDelayed(updatedHistory);
        }

        /// <summary>
        /// The update trade latest trading histories.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradingHistory">
        /// The trading history.
        /// </param>
        /// <param name="backwardWindowSize">
        /// The backward window size.
        /// </param>
        /// <param name="forwardWindowSize">
        /// The forward window size.
        /// </param>
        /// <returns>
        /// The <see cref="ITradingHistoryStack"/>.
        /// </returns>
        private ITradingHistoryStack UpdateTradeLatestTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingHistory,
            TimeSpan backwardWindowSize,
            TimeSpan? forwardWindowSize)
        {
            if (!tradingHistory.ContainsKey(order.Instrument.Identifiers))
            {
                ITradingHistoryStack history = new TradingHistoryStack(backwardWindowSize, i => i.MostRecentDateEvent(), this.tradingStackLogger);
                ITradingHistoryStack historyDecorator = new TradingHistoryDelayedDecorator(history, forwardWindowSize.GetValueOrDefault());
                var stack =
                    forwardWindowSize != null
                    ? historyDecorator
                    : history;

                stack.Add(order, order.MostRecentDateEvent());
                tradingHistory.TryAdd(order.Instrument.Identifiers, stack);
            }
            else
            {
                tradingHistory.TryGetValue(order.Instrument.Identifiers, out var history);
                history?.Add(order, order.MostRecentDateEvent());
                history?.ArchiveExpiredActiveItems(order.MostRecentDateEvent());
            }

            tradingHistory.TryGetValue(order.Instrument.Identifiers, out var updatedHistory);

            return updatedHistory;
        }

        /// <summary>
        /// The trade filled.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void TradeFilled(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (this.HasReachedFutureUniverseEpoch)
            {
                return;
            }

            if (value.FilledDate == null)
            {
                this.logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            this.logger?.LogTrace($"Trade Filled event (status changed) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                this.UpdateTradeFilledTradingHistories(
                    value, 
                    this.TradingFillsHistory,
                    this.TradeBackwardWindowSize,
                    null);

            this.RunOrderFilledEvent(updatedHistory);
        }

        /// <summary>
        /// The trade filled delay.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void TradeFilledDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (value.FilledDate == null)
            {
                this.logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            this.logger?.LogTrace($"Trade Filled event (status changed - delayed) in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                this.UpdateTradeFilledTradingHistories(
                    value,
                    this.DelayedTradingFillsHistory,
                    this.TradeBackwardWindowSize,
                    this.ForwardWindowSize);

            this.RunOrderFilledEventDelayed(updatedHistory);
        }

        /// <summary>
        /// The update trade filled trading histories.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradingFillsHistory">
        /// The trading fills history.
        /// </param>
        /// <param name="backwardWindow">
        /// The backward window.
        /// </param>
        /// <param name="forwardWindow">
        /// The forward window.
        /// </param>
        /// <returns>
        /// The <see cref="ITradingHistoryStack"/>.
        /// </returns>
        private ITradingHistoryStack UpdateTradeFilledTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingFillsHistory,
            TimeSpan backwardWindow,
            TimeSpan? forwardWindow)
        {
            if (!tradingFillsHistory.ContainsKey(order.Instrument.Identifiers))
            {
                // ReSharper disable once PossibleInvalidOperationException
                ITradingHistoryStack history = new TradingHistoryStack(backwardWindow, i => i.FilledDate.Value, this.tradingStackLogger);
                ITradingHistoryStack historyDecorator = new TradingHistoryDelayedDecorator(history, forwardWindow.GetValueOrDefault());
                var stack =
                    forwardWindow != null
                        ? historyDecorator
                        : history;

                // ReSharper disable once PossibleInvalidOperationException
                stack.Add(order, order.FilledDate.Value);
                tradingFillsHistory.TryAdd(order.Instrument.Identifiers, stack);
            }
            else
            {
                tradingFillsHistory.TryGetValue(order.Instrument.Identifiers, out var history);
                // ReSharper disable once PossibleInvalidOperationException
                history?.Add(order, order.FilledDate.Value);
                history?.ArchiveExpiredActiveItems(order.FilledDate.Value);
            }

            tradingFillsHistory.TryGetValue(order.Instrument.Identifiers, out var updatedHistory);

            return updatedHistory;
        }

        /// <summary>
        /// The market opened.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void MarketOpened(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            this.logger?.LogTrace($"Market opened event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.MarketOpen(value);
        }

        /// <summary>
        /// The market closed.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void MarketClosed(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            this.logger?.LogInformation($"Market closed event in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.MarketClose(value);
        }

        /// <summary>
        /// The eschaton.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        private void Eschaton(IUniverseEvent universeEvent)
        {
            this.logger?.LogInformation($"Eschaton in base universe rule occuring for {this.name} | event/universe time {universeEvent.EventTime}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.HasReachedEndOfUniverse = true;
            this.EndOfUniverse();
        }

        /// <summary>
        /// The run rule for all trading histories.
        /// </summary>
        /// <param name="currentTimeInUniverse">
        /// The current time in universe.
        /// </param>
        protected void RunRuleForAllTradingHistories(DateTime? currentTimeInUniverse = null)
        {
            lock (this.@lock)
            {
                this.logger?.LogInformation($"Base universe rule for {this.name} - Run rule for all trading histories {currentTimeInUniverse}");
                foreach (var history in this.TradingHistory)
                {
                    if (currentTimeInUniverse != null)
                    {
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    }

                    this.RunPostOrderEvent(history.Value);
                }
            }
        }

        /// <summary>
        /// The run rule for all trading histories in market.
        /// </summary>
        /// <param name="closeOpen">
        /// The close open.
        /// </param>
        /// <param name="currentTimeInUniverse">
        /// The current time in universe.
        /// </param>
        protected void RunRuleForAllTradingHistoriesInMarket(MarketOpenClose closeOpen, DateTime? currentTimeInUniverse = null)
        {
            lock (this.@lock)
            {
                if (closeOpen == null)
                {
                    return;
                }

                this.logger?.LogInformation($"Base universe rule for {this.name} - Run rule for all trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories =
                    this.TradingHistory
                        .Where(th =>
                            string.Equals(
                                th.Value?.Exchange()?.MarketIdentifierCode,
                                closeOpen.MarketId,
                                StringComparison.InvariantCultureIgnoreCase))
                        .ToList();

                foreach (var history in filteredTradingHistories)
                {
                    if (currentTimeInUniverse != null)
                    {
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    }

                    this.RunPostOrderEvent(history.Value);
                }
            }
        }

        /// <summary>
        /// The run rule for all delayed trading histories in market.
        /// </summary>
        /// <param name="closeOpen">
        /// The close open.
        /// </param>
        /// <param name="currentTimeInUniverse">
        /// The current time in universe.
        /// </param>
        protected void RunRuleForAllDelayedTradingHistoriesInMarket(MarketOpenClose closeOpen, DateTime? currentTimeInUniverse = null)
        {
            lock (this.@lock)
            {
                if (closeOpen == null)
                {
                    return;
                }

                this.logger?.LogInformation($"Base universe rule for {this.name} - Run rule for all delayed trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories =
                    this.DelayedTradingHistory
                        .Where(th =>
                            string.Equals(
                                th.Value?.Exchange()?.MarketIdentifierCode,
                                closeOpen.MarketId,
                                StringComparison.InvariantCultureIgnoreCase))
                        .ToList();

                foreach (var history in filteredTradingHistories)
                {
                    if (currentTimeInUniverse != null)
                    {
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    }

                    this.RunPostOrderEventDelayed(history.Value);
                }
            }
        }

        /// <summary>
        /// Run the rule with a trading history within the time window for that security.
        /// This is done on the basis of status changed on i.e. the last state and the time of
        /// that state change is used to drive run rule.
        /// </summary>
        protected abstract void RunPostOrderEvent(ITradingHistoryStack history);

        /// <summary>
        /// We have some rules such as spoofing and layering that are HFT and need to be based off
        /// of when the rule was initially submitted in order to preserve the ordering between events
        /// </summary>
        protected abstract void RunInitialSubmissionEvent(ITradingHistoryStack history);

        /// <summary>
        /// We have some rules which are based off of filled orders and we would prefer
        /// to have them ordered by their fill date/time
        /// </summary>
        public abstract void RunOrderFilledEvent(ITradingHistoryStack history);

        /// <summary>
        /// Run the rule with a trading history within the time window for that security.
        /// This is done on the basis of status changed on i.e. the last state and the time of
        /// that state change is used to drive run rule.
        /// == uses the delayed cache ==
        /// </summary>
        protected abstract void RunPostOrderEventDelayed(ITradingHistoryStack history);

        /// <summary>
        /// We have some rules such as spoofing and layering that are HFT and need to be based off
        /// of when the rule was initially submitted in order to preserve the ordering between events
        /// == uses the delayed cache ==
        /// </summary>
        protected abstract void RunInitialSubmissionEventDelayed(ITradingHistoryStack history);

        /// <summary>
        /// We have some rules which are based off of filled orders and we would prefer
        /// to have them ordered by their fill date/time
        /// == uses the delayed cache ==
        /// </summary>
        public abstract void RunOrderFilledEventDelayed(ITradingHistoryStack history);

        /// <summary>
        /// The genesis.
        /// </summary>
        protected abstract void Genesis();

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected abstract void MarketOpen(MarketOpenClose exchange);

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected abstract void MarketClose(MarketOpenClose exchange);

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected abstract void EndOfUniverse();
    }
}
