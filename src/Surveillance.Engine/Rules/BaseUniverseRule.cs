using System;
using System.Collections.Concurrent;
using System.Linq;
using Domain.Core.Financial;
using Domain.Equity.TimeBars;
using Domain.Scheduling;
using Domain.Trading;
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
        protected readonly TimeSpan WindowSize;

        protected IUniverseEquityIntradayCache UniverseEquityIntradayCache;
        protected IUniverseEquityInterDayCache UniverseEquityInterdayCache;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingHistory;
        protected ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingInitialHistory;

        protected ScheduledExecution Schedule;
        protected DateTime UniverseDateTime;
        protected bool HasReachedEndOfUniverse;
        protected readonly ISystemProcessOperationRunRuleContext RuleCtx;
        protected readonly RuleRunMode RunMode;

        private readonly ILogger _logger;
        private readonly ILogger<TradingHistoryStack> _tradingStackLogger;
        private readonly object _lock = new object();

        protected BaseUniverseRule(
            TimeSpan windowSize,
            Domain.Scheduling.Rules rules,
            string version,
            string name,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory marketCacheFactory,
            RuleRunMode runMode,
            ILogger logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
        {
            WindowSize = windowSize;
            Rule = rules;
            Version = version ?? string.Empty;

            UniverseEquityIntradayCache =
                marketCacheFactory?.BuildIntraday(windowSize, runMode)
                ?? throw new ArgumentNullException(nameof(marketCacheFactory));

            UniverseEquityInterdayCache =
                marketCacheFactory?.BuildInterday(runMode)
                ?? throw new ArgumentNullException(nameof(marketCacheFactory));

            TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
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
                        break;
                    case UniverseStateEvent.EquityInterDayTick:
                        EquityInterDay(value);
                        break;
                    case UniverseStateEvent.OrderPlaced:
                        TradeSubmitted(value);
                        break;
                    case UniverseStateEvent.Order:
                        Trade(value);
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

            _logger?.LogTrace($"Trade placed event in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key) {value.ReddeerOrderId} | placed on {value.PlacedDate}");

            UniverseDateTime = universeEvent.EventTime;

            if (!TradingInitialHistory.ContainsKey(value.Instrument.Identifiers))
            {
                var history = new TradingHistoryStack(WindowSize, i => i.PlacedDate.GetValueOrDefault(), _tradingStackLogger);
                history.Add(value, value.PlacedDate.GetValueOrDefault());
                TradingInitialHistory.TryAdd(value.Instrument.Identifiers, history);
            }
            else
            {
                TradingInitialHistory.TryGetValue(value.Instrument.Identifiers, out var history);

                history?.Add(value, value.PlacedDate.GetValueOrDefault());
                history?.ArchiveExpiredActiveItems(value.PlacedDate.GetValueOrDefault());
            }

            TradingInitialHistory.TryGetValue(value.Instrument.Identifiers, out var updatedHistory);

            RunInitialSubmissionRule(updatedHistory);
        }

        private void Trade(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            _logger?.LogTrace($"Trade event (status changed) in base universe rule occuring for {_name} | event/universe time {universeEvent.EventTime} | reddeer order id (p key){value.ReddeerOrderId}");

            UniverseDateTime = universeEvent.EventTime;

            if (!TradingHistory.ContainsKey(value.Instrument.Identifiers))
            {
                var history = new TradingHistoryStack(WindowSize, i => i.MostRecentDateEvent(), _tradingStackLogger);
                history.Add(value, value.MostRecentDateEvent());
                TradingHistory.TryAdd(value.Instrument.Identifiers, history);
            }
            else
            {
                TradingHistory.TryGetValue(value.Instrument.Identifiers, out var history);

                history?.Add(value, value.MostRecentDateEvent());
                history?.ArchiveExpiredActiveItems(value.MostRecentDateEvent());
            }

            TradingHistory.TryGetValue(value.Instrument.Identifiers, out var updatedHistory);

            RunPostOrderEvent(updatedHistory);
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
        protected abstract void RunInitialSubmissionRule(ITradingHistoryStack history);

        protected abstract void Genesis();
        protected abstract void MarketOpen(MarketOpenClose exchange);
        protected abstract void MarketClose(MarketOpenClose exchange);
        protected abstract void EndOfUniverse();

        public Domain.Scheduling.Rules Rule { get; }
        public string Version { get; }


        public void BaseClone()
        {
            UniverseEquityIntradayCache = (IUniverseEquityIntradayCache)UniverseEquityIntradayCache.Clone();
            UniverseEquityInterdayCache = (IUniverseEquityInterDayCache) UniverseEquityInterdayCache.Clone();
            TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(TradingHistory);
            TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>(TradingInitialHistory);
        }
    }
}
