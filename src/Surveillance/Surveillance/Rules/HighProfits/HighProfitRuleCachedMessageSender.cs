using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits
{
    /// <summary>
    /// Cache and deduplicate high profit rule messages
    /// To send the messages onto the bus explicitly call Flush
    /// </summary>
    public class HighProfitRuleCachedMessageSender : IHighProfitRuleCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly IHighProfitMessageSender _messageSender;
        private readonly ILogger<HighProfitRuleCachedMessageSender> _logger;
        private List<IHighProfitRuleBreach> _messages;

        public HighProfitRuleCachedMessageSender(
            IHighProfitMessageSender messageSender,
            ILogger<HighProfitRuleCachedMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IHighProfitRuleBreach>();
        }

        /// <summary>
        /// Receive and cache rule breach in memory
        /// </summary>
        public void Send(IHighProfitRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"High Profit Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                _messages = _messages.Except(duplicates).ToList();

                _logger.LogInformation($"High Profit Rule Cached Message Sender deduplicating {_messages.Count()} for {ruleBreach.Security.Identifiers}");

                _messages.Add(ruleBreach);
            }
        }

        public void Remove(ITradePosition position)
        {
            if (position == null
                || !position.Get().Any())
            {
                return;
            }

            lock (_lock)
            {
                var duplicates =
                    _messages
                        .Where(msg => msg != null)
                        .Where(msg => msg.MarketClosureVirtualProfitComponent)
                        .Where(msg => msg.Trades.PositionIsSubsetOf(position))
                        .ToList();

                _messages = _messages.Except(duplicates).ToList();
            }
        }

        /// <summary>
        /// Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush(ISystemProcessOperationRunRuleContext ruleCtx)
        {
            lock (_lock)
            {
                _logger.LogInformation($"High Profit Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _logger.LogInformation($"High Profit Rule Cached Message Sender dispatching {msg?.Security?.Identifiers} rule breaches to message bus");
                    _messageSender.Send(msg, ruleCtx);
                }

                var count = _messages.Count;
                _messages.RemoveAll(m => true);

                return count;
            }
        }

        public void Delete()
        {
            lock (_lock)
            {
                _logger.LogInformation($"High Profit Rule Cached Message Sender deleting alert messages");
                _messages = new List<IHighProfitRuleBreach>();
            }
        }
    }
}
