using System;
using System.Collections.Concurrent;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Stub;
using Surveillance.Rules.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Rules
{
    public abstract class BaseUniverseRule : IUniverseRule
    {
        private readonly string _name;

        protected readonly TimeSpan WindowSize;
        protected readonly ConcurrentDictionary<SecurityIdentifiers, ITradingHistoryStack> TradingHistory;

        private readonly ILogger _logger;
        private readonly object _lock = new object();

        protected ExchangeFrame LatestExchangeFrame;
        protected ScheduledExecution Schedule;
        protected bool HasReachedEndOfUniverse;

        protected BaseUniverseRule(
            TimeSpan windowSize,
            Domain.Scheduling.Rules rules,
            string version,
            string name,
            ILogger logger)
        {
            WindowSize = windowSize;
            Rule = rules;
            Version = version ?? string.Empty;
            TradingHistory = new ConcurrentDictionary<SecurityIdentifiers, ITradingHistoryStack>();

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
                        Eschaton();
                        break;
                    case UniverseStateEvent.Unknown:
                        _logger.LogWarning($"Universe rule {_name} received an unknown event");
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
            Genesis();
        }

        private void StockTick(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is ExchangeFrame value))
            {
                return;
            }

            LatestExchangeFrame = value;
        }

        private void Trade(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is TradeOrderFrame value))
            {
                return;
            }

            if (!TradingHistory.ContainsKey(value.Security.Identifiers))
            {
                var history = new TradingHistoryStack(WindowSize);
                history.Add(value, value.StatusChangedOn);
                TradingHistory.TryAdd(value.Security.Identifiers, history);
            }
            else
            {
                TradingHistory.TryGetValue(value.Security.Identifiers, out var history);

                history?.Add(value, value.StatusChangedOn);
                history?.ArchiveExpiredActiveItems(value.StatusChangedOn);
            }

            TradingHistory.TryGetValue(value.Security.Identifiers, out var updatedHistory);

            RunRule(updatedHistory);
        }

        private void MarketOpened(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            MarketOpen(value);
        }

        private void MarketClosed(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is MarketOpenClose value))
            {
                return;
            }

            MarketClose(value);
        }

        private void Eschaton()
        {
            HasReachedEndOfUniverse = true;
            EndOfUniverse();
        }

        protected void RunRuleForAllTradingHistories()
        {
            lock (_lock)
            {
                foreach (var history in TradingHistory)
                {
                    RunRule(history.Value);
                }
            }
        }

        /// <summary>
        /// Run the rule with a trading history within the time window for that security
        /// </summary>
        protected abstract void RunRule(ITradingHistoryStack history);

        protected abstract void Genesis();
        protected abstract void MarketOpen(MarketOpenClose exchange);
        protected abstract void MarketClose(MarketOpenClose exchange);
        protected abstract void EndOfUniverse();

        public Domain.Scheduling.Rules Rule { get; }
        public string Version { get; }
    }
}
