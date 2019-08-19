

// ReSharper disable InconsistentlySynchronizedField
namespace Surveillance.Engine.Rules.Rules
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public abstract class BaseUniverseRule : IUniverseRule
    {
        protected readonly TimeSpan BackwardWindowSize;

        protected readonly TimeSpan ForwardWindowSize;

        protected readonly ISystemProcessOperationRunRuleContext RuleCtx;

        protected readonly RuleRunMode RunMode;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingFillsHistory;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingHistory;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingInitialHistory;

        /// <summary>
        ///     These are paid up with the delayed trading histories to create the illusion of future data analysis
        ///     whilst maintaining a singular set of abstractions around the backward aspect of analysis
        /// </summary>
        protected IUniverseEquityIntradayCache FutureUniverseEquityIntradayCache;

        protected bool HasReachedEndOfUniverse;

        protected bool HasReachedFutureUniverseEpoch;

        protected ScheduledExecution Schedule;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingFillsHistory;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingHistory;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingInitialHistory;

        protected DateTime UniverseDateTime;

        protected IUniverseEquityInterDayCache UniverseEquityInterdayCache;

        protected IUniverseEquityIntradayCache UniverseEquityIntradayCache;

        protected IUniverseEvent UniverseEvent;

        private readonly object _lock = new object();

        private readonly ILogger _logger;

        private readonly string _name;

        private readonly ILogger<TradingHistoryStack> _tradingStackLogger;

        protected BaseUniverseRule(
            TimeSpan backwardWindowSize,
            TimeSpan forwardWindowSize,
            Rules rules,
            string version,
            string name,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
        {
            this.BackwardWindowSize = backwardWindowSize;
            this.ForwardWindowSize = forwardWindowSize;

            this.Rule = rules;
            this.Version = version ?? string.Empty;

            this.UniverseEquityIntradayCache = factory?.BuildIntraday(backwardWindowSize, runMode)
                                               ?? throw new ArgumentNullException(nameof(factory));

            this.FutureUniverseEquityIntradayCache = factory?.BuildIntraday(forwardWindowSize, runMode)
                                                     ?? throw new ArgumentNullException(nameof(factory));

            this.UniverseEquityInterdayCache =
                factory?.BuildInterday(runMode) ?? throw new ArgumentNullException(nameof(factory));

            this.TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.TradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            this.DelayedTradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.DelayedTradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            this.DelayedTradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            this.RuleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            this._name = name ?? "Unnamed rule";
            this.RunMode = runMode;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tradingStackLogger =
                tradingStackLogger ?? throw new ArgumentNullException(nameof(tradingStackLogger));
        }

        public Rules Rule { get; }

        public string Version { get; }

        public void BaseClone()
        {
            this.UniverseEquityIntradayCache = (IUniverseEquityIntradayCache)this.UniverseEquityIntradayCache.Clone();
            this.UniverseEquityInterdayCache = (IUniverseEquityInterDayCache)this.UniverseEquityInterdayCache.Clone();
            this.TradingHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingHistory);
            this.TradingFillsHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingFillsHistory);
            this.TradingInitialHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.TradingInitialHistory);
            this.DelayedTradingHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.DelayedTradingHistory);
            this.DelayedTradingFillsHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(this.DelayedTradingFillsHistory);
            this.DelayedTradingInitialHistory =
                new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(
                    this.DelayedTradingInitialHistory);
        }

        public void OnCompleted()
        {
            this._logger?.LogInformation($"Universe Rule {this._name} completed its universe stream");
        }

        public void OnError(Exception error)
        {
            this._logger?.LogError($"{this._name} {this.Version} {error}");
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null) return;

            var filteredValue = this.Filter(value);
            if (filteredValue == null)
            {
                this._logger?.LogInformation($"base universe event at {value.EventTime} filtered out. Skipping event.");
                return;
            }

            this.UniverseEvent = value;
            this._logger?.LogTrace(
                $"{value} universe event passed to {this._name} at universe time {value.EventTime}.");

            lock (this._lock)
            {
                switch (value.StateChange)
                {
                    case UniverseStateEvent.Genesis:
                        this.Genesis(value);
                        break;
                    case UniverseStateEvent.EquityIntradayTick:
                        this.EquityIntraDay(value);
                        this.FutureEquityIntraDay(value);
                        break;
                    case UniverseStateEvent.EquityInterDayTick:
                        this.EquityInterDay(value);
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
                        this._logger?.LogWarning($"Universe rule {this._name} received an unknown event");
                        this.RuleCtx.EventException($"Universe rule {this._name} received an unknown event");
                        break;
                    case UniverseStateEvent.EpochFutureUniverse:
                        this.HasReachedFutureUniverseEpoch = true;
                        break;
                }
            }
        }

        /// <summary>
        ///     We have some rules which are based off of filled orders and we would prefer
        ///     to have them ordered by their fill date/time
        /// </summary>
        public abstract void RunOrderFilledEvent(ITradingHistoryStack history);

        /// <summary>
        ///     We have some rules which are based off of filled orders and we would prefer
        ///     to have them ordered by their fill date/time
        ///     == uses the delayed cache ==
        /// </summary>
        public abstract void RunOrderFilledEventDelayed(ITradingHistoryStack history);

        protected abstract void EndOfUniverse();

        protected abstract IUniverseEvent Filter(IUniverseEvent value);

        protected abstract void Genesis();

        protected abstract void MarketClose(MarketOpenClose exchange);

        protected abstract void MarketOpen(MarketOpenClose exchange);

        /// <summary>
        ///     We have some rules such as spoofing and layering that are HFT and need to be based off
        ///     of when the rule was initially submitted in order to preserve the ordering between events
        /// </summary>
        protected abstract void RunInitialSubmissionEvent(ITradingHistoryStack history);

        /// <summary>
        ///     We have some rules such as spoofing and layering that are HFT and need to be based off
        ///     of when the rule was initially submitted in order to preserve the ordering between events
        ///     == uses the delayed cache ==
        /// </summary>
        protected abstract void RunInitialSubmissionEventDelayed(ITradingHistoryStack history);

        /// <summary>
        ///     Run the rule with a trading history within the time window for that security.
        ///     This is done on the basis of status changed on i.e. the last state and the time of
        ///     that state change is used to drive run rule.
        /// </summary>
        protected abstract void RunPostOrderEvent(ITradingHistoryStack history);

        /// <summary>
        ///     Run the rule with a trading history within the time window for that security.
        ///     This is done on the basis of status changed on i.e. the last state and the time of
        ///     that state change is used to drive run rule.
        ///     == uses the delayed cache ==
        /// </summary>
        protected abstract void RunPostOrderEventDelayed(ITradingHistoryStack history);

        protected void RunRuleForAllDelayedTradingHistoriesInMarket(
            MarketOpenClose closeOpen,
            DateTime? currentTimeInUniverse = null)
        {
            lock (this._lock)
            {
                if (closeOpen == null) return;

                this._logger?.LogInformation(
                    $"Base universe rule for {this._name} - Run rule for all delayed trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories = this.DelayedTradingHistory.Where(
                    th => string.Equals(
                        th.Value?.Exchange()?.MarketIdentifierCode,
                        closeOpen.MarketId,
                        StringComparison.InvariantCultureIgnoreCase)).ToList();

                foreach (var history in filteredTradingHistories)
                {
                    if (currentTimeInUniverse != null)
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);

                    this.RunPostOrderEventDelayed(history.Value);
                }
            }
        }

        protected void RunRuleForAllTradingHistories(DateTime? currentTimeInUniverse = null)
        {
            lock (this._lock)
            {
                this._logger?.LogInformation(
                    $"Base universe rule for {this._name} - Run rule for all trading histories {currentTimeInUniverse}");
                foreach (var history in this.TradingHistory)
                {
                    if (currentTimeInUniverse != null)
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    this.RunPostOrderEvent(history.Value);
                }
            }
        }

        protected void RunRuleForAllTradingHistoriesInMarket(
            MarketOpenClose closeOpen,
            DateTime? currentTimeInUniverse = null)
        {
            lock (this._lock)
            {
                if (closeOpen == null) return;

                this._logger?.LogInformation(
                    $"Base universe rule for {this._name} - Run rule for all trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories = this.TradingHistory.Where(
                    th => string.Equals(
                        th.Value?.Exchange()?.MarketIdentifierCode,
                        closeOpen.MarketId,
                        StringComparison.InvariantCultureIgnoreCase)).ToList();

                foreach (var history in filteredTradingHistories)
                {
                    if (currentTimeInUniverse != null)
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    this.RunPostOrderEvent(history.Value);
                }
            }
        }

        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value)) return;

            this._logger?.LogInformation(
                $"Equity inter day event in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseEquityInterdayCache.Add(value);
        }

        private void EquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value)) return;

            this._logger?.LogInformation(
                $"Equity intra day event in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.UniverseEquityIntradayCache.Add(value);
        }

        private void Eschaton(IUniverseEvent universeEvent)
        {
            this._logger?.LogInformation(
                $"Eschaton in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.HasReachedEndOfUniverse = true;
            this.EndOfUniverse();
        }

        private void FutureEquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value)) return;

            this._logger?.LogInformation(
                $"Equity intra day event (future) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.FutureUniverseEquityIntradayCache.Add(value);
        }

        private void Genesis(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is ScheduledExecution value)) return;

            this._logger?.LogInformation(
                $"Genesis event in base universe rule occuring for rule {this._name} | event/universe time {universeEvent.EventTime} | correlation id {value.CorrelationId} | time series initiation  {value.TimeSeriesInitiation} | time series termination {value.TimeSeriesTermination}");

            this.Schedule = value;
            this.UniverseDateTime = universeEvent.EventTime;
            this.Genesis();
        }

        private void MarketClosed(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value)) return;

            this._logger?.LogInformation(
                $"Market closed event in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.MarketClose(value);
        }

        private void MarketOpened(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value)) return;

            this._logger?.LogTrace(
                $"Market opened event in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            this.UniverseDateTime = universeEvent.EventTime;
            this.MarketOpen(value);
        }

        private void Trade(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            if (this.HasReachedFutureUniverseEpoch)
                return;

            this._logger?.LogTrace(
                $"Trade event (status changed) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeLatestTradingHistories(
                value,
                this.TradingHistory,
                this.BackwardWindowSize,
                null);

            this.RunPostOrderEvent(updatedHistory);
        }

        private void TradeDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            this._logger?.LogTrace(
                $"Trade event (status changed delayed) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeLatestTradingHistories(
                value,
                this.DelayedTradingHistory,
                this.BackwardWindowSize,
                this.ForwardWindowSize);

            this.RunPostOrderEventDelayed(updatedHistory);
        }

        private void TradeFilled(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            if (this.HasReachedFutureUniverseEpoch)
                return;

            if (value.FilledDate == null)
            {
                this._logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            this._logger?.LogTrace(
                $"Trade Filled event (status changed) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeFilledTradingHistories(
                value,
                this.TradingFillsHistory,
                this.BackwardWindowSize,
                null);

            this.RunOrderFilledEvent(updatedHistory);
        }

        private void TradeFilledDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            if (value.FilledDate == null)
            {
                this._logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            this._logger?.LogTrace(
                $"Trade Filled event (status changed - delayed) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeFilledTradingHistories(
                value,
                this.DelayedTradingFillsHistory,
                this.BackwardWindowSize,
                this.ForwardWindowSize);

            this.RunOrderFilledEventDelayed(updatedHistory);
        }

        private void TradeSubmitted(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            if (this.HasReachedFutureUniverseEpoch)
                return;

            this._logger?.LogTrace(
                $"Trade placed event in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeSubmittedTradingHistories(
                value,
                this.TradingInitialHistory,
                this.BackwardWindowSize,
                null);

            this.RunInitialSubmissionEvent(updatedHistory);
        }

        private void TradeSubmittedDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value)) return;

            this._logger?.LogTrace(
                $"Trade placed event (delay) in base universe rule occuring for {this._name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            this.UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = this.UpdateTradeSubmittedTradingHistories(
                value,
                this.DelayedTradingInitialHistory,
                this.BackwardWindowSize,
                this.ForwardWindowSize);

            this.RunInitialSubmissionEventDelayed(updatedHistory);
        }

        private ITradingHistoryStack UpdateTradeFilledTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingFillsHistory,
            TimeSpan backwardWindow,
            TimeSpan? forwardWindow)
        {
            if (!tradingFillsHistory.ContainsKey(order.Instrument.Identifiers))
            {
                // ReSharper disable once PossibleInvalidOperationException
                ITradingHistoryStack history = new TradingHistoryStack(
                    backwardWindow,
                    i => i.FilledDate.Value,
                    this._tradingStackLogger);
                ITradingHistoryStack historyDecorator =
                    new TradingHistoryDelayedDecorator(history, forwardWindow.GetValueOrDefault());
                var stack = forwardWindow != null ? historyDecorator : history;

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

        private ITradingHistoryStack UpdateTradeLatestTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingHistory,
            TimeSpan backwardWindowSize,
            TimeSpan? forwardWindowSize)
        {
            if (!tradingHistory.ContainsKey(order.Instrument.Identifiers))
            {
                ITradingHistoryStack history = new TradingHistoryStack(
                    backwardWindowSize,
                    i => i.MostRecentDateEvent(),
                    this._tradingStackLogger);
                ITradingHistoryStack historyDecorator = new TradingHistoryDelayedDecorator(
                    history,
                    forwardWindowSize.GetValueOrDefault());
                var stack = forwardWindowSize != null ? historyDecorator : history;

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

        private ITradingHistoryStack UpdateTradeSubmittedTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingHistory,
            TimeSpan backwardWindowSize,
            TimeSpan? forwardWindowSize)
        {
            if (!tradingHistory.ContainsKey(order.Instrument.Identifiers))
            {
                ITradingHistoryStack history = new TradingHistoryStack(
                    backwardWindowSize,
                    i => i.PlacedDate.GetValueOrDefault(),
                    this._tradingStackLogger);

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
    }
}