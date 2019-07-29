﻿using System;
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

// ReSharper disable InconsistentlySynchronizedField
namespace Surveillance.Engine.Rules.Rules
{
    public abstract class BaseUniverseRule : IUniverseRule
    {
        private readonly string _name;
        protected readonly TimeSpan BackwardWindowSize;
        protected readonly TimeSpan ForwardWindowSize;

        protected IUniverseEquityIntradayCache UniverseEquityIntradayCache;
        protected IUniverseEquityInterDayCache UniverseEquityInterdayCache;

        protected IUniverseEvent UniverseEvent;

        /// <summary>
        /// These are paid up with the delayed trading histories to create the illusion of future data analysis
        /// whilst maintaining a singular set of abstractions around the backward aspect of analysis
        /// </summary>
        protected IUniverseEquityIntradayCache FutureUniverseEquityIntradayCache;

        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingFillsHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingInitialHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingFillsHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> DelayedTradingInitialHistory;

        protected ScheduledExecution Schedule;
        protected DateTime UniverseDateTime;
        protected bool HasReachedEndOfUniverse;
        protected bool HasReachedFutureUniverseEpoch;
        protected readonly ISystemProcessOperationRunRuleContext RuleCtx;
        protected readonly RuleRunMode RunMode;

        private readonly ILogger _logger;
        private readonly ILogger<TradingHistoryStack> _tradingStackLogger;
        private readonly object _lock = new object();

        protected BaseUniverseRule(
            TimeSpan backwardWindowSize,
            TimeSpan forwardWindowSize,
            Domain.Surveillance.Scheduling.Rules rules,
            string version,
            string name,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
        {
            BackwardWindowSize = backwardWindowSize;
            ForwardWindowSize = forwardWindowSize;

            Rule = rules;
            Version = version ?? string.Empty;

            UniverseEquityIntradayCache =
                factory?.BuildIntraday(backwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(factory));

            FutureUniverseEquityIntradayCache =
                factory?.BuildIntraday(forwardWindowSize, runMode)
                ?? throw new ArgumentNullException(nameof(factory));

            UniverseEquityInterdayCache =
                factory?.BuildInterday(runMode)
                ?? throw new ArgumentNullException(nameof(factory));

            TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            TradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            DelayedTradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            DelayedTradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            DelayedTradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();

            RuleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _name = name ?? "Unnamed rule";
            RunMode = runMode;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingStackLogger = tradingStackLogger ?? throw new ArgumentNullException(nameof(tradingStackLogger));
        }

        public void OnCompleted()
        {
            _logger?.LogInformation($"Universe Rule {_name} completed its universe stream");
        }

        public void OnError(Exception error)
        {
            _logger?.LogError($"{_name} {Version} {error}");
        }

        protected abstract IUniverseEvent Filter(IUniverseEvent value);

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            var filteredValue = Filter(value);
            if (filteredValue == null)
            {
                _logger?.LogInformation($"base universe event at {value.EventTime} filtered out. Skipping event.");
                return;
            }

            UniverseEvent = value;
            _logger?.LogTrace($"{value} universe event passed to {_name} at universe time {value.EventTime}.");

            lock (_lock)
            {
                switch (value.StateChange)
                {
                    case UniverseStateEvent.Genesis:
                        Genesis(value);
                        break;
                    case UniverseStateEvent.EquityIntradayTick:
                        EquityIntraDay(value);
                        FutureEquityIntraDay(value);
                        break;
                    case UniverseStateEvent.EquityInterDayTick:
                        EquityInterDay(value);
                        break;
                    case UniverseStateEvent.OrderPlaced:
                        TradeSubmitted(value);
                        TradeSubmittedDelay(value);
                        break;
                    case UniverseStateEvent.Order:
                        Trade(value);
                        TradeDelay(value);
                        break;
                    case UniverseStateEvent.OrderFilled:
                        TradeFilled(value);
                        TradeFilledDelay(value);
                        break;
                    case UniverseStateEvent.ExchangeOpen:
                        MarketOpened(value);
                        break;
                    case UniverseStateEvent.ExchangeClose:
                        MarketClosed(value);
                        break;
                    case UniverseStateEvent.Eschaton:
                        Eschaton(value);
                        break;
                    case UniverseStateEvent.Unknown:
                        _logger?.LogWarning($"Universe rule {_name} received an unknown event");
                        RuleCtx.EventException($"Universe rule {_name} received an unknown event");
                        break;
                    case UniverseStateEvent.EpochFutureUniverse:
                        HasReachedFutureUniverseEpoch = true;
                        break;
                }
            }
        }

        private void Genesis(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is ScheduledExecution value))
            {
                return;
            }

            _logger?.LogInformation($"Genesis event in base universe rule occuring for rule {_name} | event/universe time {universeEvent.EventTime} | correlation id {value.CorrelationId} | time series initiation  {value.TimeSeriesInitiation} | time series termination {value.TimeSeriesTermination}");

            Schedule = value;
            UniverseDateTime = universeEvent.EventTime;
            Genesis();
        }

        private void EquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value))
            {
                return;
            }

            _logger?.LogInformation($"Equity intra day event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            UniverseDateTime = universeEvent.EventTime;
            UniverseEquityIntradayCache.Add(value);
        }

        private void FutureEquityIntraDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityIntraDayTimeBarCollection value))
            {
                return;
            }

            _logger?.LogInformation($"Equity intra day event (future) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            UniverseDateTime = universeEvent.EventTime;
            FutureUniverseEquityIntradayCache.Add(value);
        }

        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value))
            {
                return;
            }

            _logger?.LogInformation($"Equity inter day event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            UniverseDateTime = universeEvent.EventTime;
            UniverseEquityInterdayCache.Add(value);
        }


        private void TradeSubmitted(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (HasReachedFutureUniverseEpoch)
                return;

            _logger?.LogTrace($"Trade placed event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                UpdateTradeSubmittedTradingHistories(
                    value,
                    TradingInitialHistory,
                    BackwardWindowSize,
                    null);

            RunInitialSubmissionEvent(updatedHistory);
        }

        private void TradeSubmittedDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            _logger?.LogTrace($"Trade placed event (delay) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory =
                UpdateTradeSubmittedTradingHistories(
                    value,
                    DelayedTradingInitialHistory,
                    BackwardWindowSize,
                    ForwardWindowSize);

            RunInitialSubmissionEventDelayed(updatedHistory);
        }

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
                        _tradingStackLogger);

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

        private void Trade(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (HasReachedFutureUniverseEpoch)
                return;

            _logger?.LogTrace($"Trade event (status changed) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = UpdateTradeLatestTradingHistories(value, TradingHistory, BackwardWindowSize, null);

            RunPostOrderEvent(updatedHistory);
        }

        private void TradeDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            _logger?.LogTrace($"Trade event (status changed delayed) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = UpdateTradeLatestTradingHistories(value, DelayedTradingHistory, BackwardWindowSize, ForwardWindowSize);

            RunPostOrderEventDelayed(updatedHistory);
        }

        private ITradingHistoryStack UpdateTradeLatestTradingHistories(
            Order order,
            ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> tradingHistory,
            TimeSpan backwardWindowSize,
            TimeSpan? forwardWindowSize)
        {
            if (!tradingHistory.ContainsKey(order.Instrument.Identifiers))
            {
                ITradingHistoryStack history = new TradingHistoryStack(backwardWindowSize, i => i.MostRecentDateEvent(), _tradingStackLogger);
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
        
        private void TradeFilled(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (HasReachedFutureUniverseEpoch)
                return;

            if (value.FilledDate == null)
            {
                _logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            _logger?.LogTrace($"Trade Filled event (status changed) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = UpdateTradeFilledTradingHistories(value, TradingFillsHistory, BackwardWindowSize, null);

            RunOrderFilledEvent(updatedHistory);
        }

        private void TradeFilledDelay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            if (value.FilledDate == null)
            {
                _logger?.LogError($"Trade filled with null fill date {value.Instrument.Identifiers}");
                return;
            }

            _logger?.LogTrace($"Trade Filled event (status changed - delayed) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            UniverseDateTime = universeEvent.EventTime;
            var updatedHistory = UpdateTradeFilledTradingHistories(value, DelayedTradingFillsHistory, BackwardWindowSize, ForwardWindowSize);

            RunOrderFilledEventDelayed(updatedHistory);
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
                ITradingHistoryStack history = new TradingHistoryStack(backwardWindow, i => i.FilledDate.Value, _tradingStackLogger);
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

        private void MarketOpened(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            _logger?.LogTrace($"Market opened event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            UniverseDateTime = universeEvent.EventTime;
            MarketOpen(value);
        }

        private void MarketClosed(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            _logger?.LogInformation($"Market closed event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | MIC {value.MarketId} | Open {value.MarketOpen} | Close {value.MarketClose}");

            UniverseDateTime = universeEvent.EventTime;
            MarketClose(value);
        }

        private void Eschaton(IUniverseEvent universeEvent)
        {
            _logger?.LogInformation($"Eschaton in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime}");

            UniverseDateTime = universeEvent.EventTime;
            HasReachedEndOfUniverse = true;
            EndOfUniverse();
        }

        protected void RunRuleForAllTradingHistories(DateTime? currentTimeInUniverse = null)
        {
            lock (_lock)
            {
                _logger?.LogInformation($"Base universe rule for {_name} - Run rule for all trading histories {currentTimeInUniverse}");
                foreach (var history in TradingHistory)
                {
                    if (currentTimeInUniverse != null)
                    {
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    }
                    RunPostOrderEvent(history.Value);
                }
            }
        }

        protected void RunRuleForAllTradingHistoriesInMarket(MarketOpenClose closeOpen, DateTime? currentTimeInUniverse = null)
        {
            lock (_lock)
            {
                if (closeOpen == null)
                {
                    return;
                }

                _logger?.LogInformation($"Base universe rule for {_name} - Run rule for all trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories =
                    TradingHistory
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
                    RunPostOrderEvent(history.Value);
                }
            }
        }

        protected void RunRuleForAllDelayedTradingHistoriesInMarket(MarketOpenClose closeOpen, DateTime? currentTimeInUniverse = null)
        {
            lock (_lock)
            {
                if (closeOpen == null)
                {
                    return;
                }

                _logger?.LogInformation($"Base universe rule for {_name} - Run rule for all delayed trading histories for the market {closeOpen.MarketId} {currentTimeInUniverse}");

                var filteredTradingHistories =
                    DelayedTradingHistory
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

                    RunPostOrderEventDelayed(history.Value);
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

        protected abstract void Genesis();
        protected abstract void MarketOpen(MarketOpenClose exchange);
        protected abstract void MarketClose(MarketOpenClose exchange);
        protected abstract void EndOfUniverse();

        public Domain.Surveillance.Scheduling.Rules Rule { get; }
        public string Version { get; }

        public void BaseClone()
        {
            UniverseEquityIntradayCache = (IUniverseEquityIntradayCache)UniverseEquityIntradayCache.Clone();
            UniverseEquityInterdayCache = (IUniverseEquityInterDayCache) UniverseEquityInterdayCache.Clone();
            TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(TradingHistory);
            TradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(TradingFillsHistory);
            TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(TradingInitialHistory);
            DelayedTradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(DelayedTradingHistory);
            DelayedTradingFillsHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(DelayedTradingFillsHistory);
            DelayedTradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(DelayedTradingInitialHistory);
        }
    }
}
