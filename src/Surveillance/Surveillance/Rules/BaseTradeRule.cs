using System;
using System.Collections.Concurrent;
using Domain.Equity;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules
{
    public abstract class BaseTradeRule : ITradeRule
    {
        protected readonly TimeSpan WindowSize;
        protected readonly ConcurrentDictionary<SecurityIdentifiers, ITradingHistoryStack> TradingHistory;

        private readonly ILogger _logger;
        private readonly object _lock = new object();

        protected BaseTradeRule(
            TimeSpan windowSize,
            Domain.Scheduling.Rules rule,
            string version,
            ILogger logger)
        {
            Rule = rule;
            Version = version ?? string.Empty;

            WindowSize = windowSize;
            _logger = logger;
            TradingHistory = new ConcurrentDictionary<SecurityIdentifiers, ITradingHistoryStack>();
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"{Rule} {Version} stream completed");
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"An error occured in the {Rule} {Version} trade stream {error}");
        }

        public void OnNext(TradeOrderFrame value)
        {
            if (value?.Security == null)
            {
                return;
            }

            lock (_lock)
            {
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
        }
        
        /// <summary>
        /// Run the rule with a trading history within the time window for that security
        /// </summary>
        /// <param name="history"></param>
        protected abstract void RunRule(ITradingHistoryStack history);

        public Domain.Scheduling.Rules Rule { get; }

        public string Version { get; }
    }
}
