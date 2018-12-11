using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using DomainV2.Scheduling;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules
{
    public abstract class BaseUniverseRule : IUniverseRule
    {
        private readonly string _name;

        protected readonly TimeSpan WindowSize;
        protected readonly ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingHistory;
        protected readonly ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack> TradingInitialHistory;
        protected readonly ISystemProcessOperationRunRuleContext RuleCtx;

        private readonly ILogger _logger;
        private readonly object _lock = new object();

        protected IDictionary<string, ExchangeFrame> LatestExchangeFrameBook;

        protected ConcurrentDictionary<string, IMarketHistoryStack> MarketHistory;

        protected ScheduledExecution Schedule;
        protected DateTime UniverseDateTime;
        protected bool HasReachedEndOfUniverse;

        protected BaseUniverseRule(
            TimeSpan windowSize,
            DomainV2.Scheduling.Rules rules,
            string version,
            string name,
            ISystemProcessOperationRunRuleContext ruleCtx,
            ILogger logger)
        {
            WindowSize = windowSize;
            Rule = rules;
            Version = version ?? string.Empty;
            TradingHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            TradingInitialHistory = new ConcurrentDictionary<InstrumentIdentifiers, ITradingHistoryStack>();
            MarketHistory = new ConcurrentDictionary<string, IMarketHistoryStack>();
            LatestExchangeFrameBook = new ConcurrentDictionary<string, ExchangeFrame>();

            RuleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _name = name ?? "Unnamed rule";
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger?.LogInformation($"Universe Rule {_name} completed its universe stream");
        }

        public void OnError(Exception error)
        {
            _logger?.LogError($"{_name} {Version} {error}");
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            lock (_lock)
            {
                switch (value.StateChange)
                {
                    case UniverseStateEvent.Genesis:
                        Genesis(value);
                        break;
                    case UniverseStateEvent.StockTickReddeer:
                        StockTick(value);
                        break;
                    case UniverseStateEvent.TradeReddeerSubmitted:
                        TradeSubmitted(value);
                        break;
                    case UniverseStateEvent.TradeReddeer:
                        Trade(value);
                        break;
                    case UniverseStateEvent.StockMarketOpen:
                        MarketOpened(value);
                        break;
                    case UniverseStateEvent.StockMarketClose:
                        MarketClosed(value);
                        break;
                    case UniverseStateEvent.Eschaton:
                        Eschaton(value);
                        break;
                    case UniverseStateEvent.Unknown:
                        _logger.LogWarning($"Universe rule {_name} received an unknown event");
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

            Schedule = value;
            UniverseDateTime = universeEvent.EventTime;
            Genesis();
        }

        private void StockTick(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is ExchangeFrame value))
            {
                return;
            }

            if (LatestExchangeFrameBook.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                LatestExchangeFrameBook.Remove(value.Exchange.MarketIdentifierCode);
                LatestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }
            else
            {
                LatestExchangeFrameBook.Add(value.Exchange.MarketIdentifierCode, value);
            }

            UniverseDateTime = universeEvent.EventTime;

            if (!MarketHistory.ContainsKey(value.Exchange.MarketIdentifierCode))
            {
                var history = new MarketHistoryStack(WindowSize);
                history.Add(value, value.TimeStamp);
                MarketHistory.TryAdd(value.Exchange.MarketIdentifierCode, history);
            }
            else
            {
                MarketHistory.TryGetValue(value.Exchange.MarketIdentifierCode, out var history);

                history?.Add(value, value.TimeStamp);
                history?.ArchiveExpiredActiveItems(value.TimeStamp);
            }
        }

        private void TradeSubmitted(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is Order value))
            {
                return;
            }

            UniverseDateTime = universeEvent.EventTime;

            if (!TradingInitialHistory.ContainsKey(value.Instrument.Identifiers))
            {
                var history = new TradingHistoryStack(WindowSize, i => i.OrderPlacedDate.GetValueOrDefault());
                history.Add(value, value.OrderPlacedDate.GetValueOrDefault());
                TradingInitialHistory.TryAdd(value.Instrument.Identifiers, history);
            }
            else
            {
                TradingInitialHistory.TryGetValue(value.Instrument.Identifiers, out var history);

                history?.Add(value, value.OrderPlacedDate.GetValueOrDefault());
                history?.ArchiveExpiredActiveItems(value.OrderPlacedDate.GetValueOrDefault());
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

            UniverseDateTime = universeEvent.EventTime;

            if (!TradingHistory.ContainsKey(value.Instrument.Identifiers))
            {
                var history = new TradingHistoryStack(WindowSize, i => i.MostRecentDateEvent());
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

            RunRule(updatedHistory);
        }

        private void MarketOpened(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            UniverseDateTime = universeEvent.EventTime;
            MarketOpen(value);
        }

        private void MarketClosed(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            UniverseDateTime = universeEvent.EventTime;
            MarketClose(value);
        }

        private void Eschaton(IUniverseEvent universeEvent)
        {
            UniverseDateTime = universeEvent.EventTime;
            HasReachedEndOfUniverse = true;
            EndOfUniverse();
        }

        protected void RunRuleForAllTradingHistories(DateTime? currentTimeInUniverse = null)
        {
            lock (_lock)
            {
                foreach (var history in TradingHistory)
                {
                    if (currentTimeInUniverse != null)
                    {
                        history.Value.ArchiveExpiredActiveItems(currentTimeInUniverse.Value);
                    }
                    RunRule(history.Value);
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
                    RunRule(history.Value);
                }
            }
        }

        /// <summary>
        /// Run the rule with a trading history within the time window for that security.
        /// This is done on the basis of status changed on i.e. the last state and the time of
        /// that state change is used to drive run rule.
        /// </summary>
        protected abstract void RunRule(ITradingHistoryStack history);

        /// <summary>
        /// We have some rules such as spoofing and layering that are HFT and need to be based off
        /// of when the rule was initially submitted in order to preserve the ordering between events
        /// </summary>
        protected abstract void RunInitialSubmissionRule(ITradingHistoryStack history);

        protected abstract void Genesis();
        protected abstract void MarketOpen(MarketOpenClose exchange);
        protected abstract void MarketClose(MarketOpenClose exchange);
        protected abstract void EndOfUniverse();

        public DomainV2.Scheduling.Rules Rule { get; }
        public string Version { get; }
    }
}
