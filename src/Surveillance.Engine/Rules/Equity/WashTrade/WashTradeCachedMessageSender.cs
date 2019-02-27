using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    public class WashTradeCachedMessageSender : IWashTradeCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly IWashTradeRuleMessageSender _messageSender;
        private readonly ILogger<WashTradeCachedMessageSender> _logger;
        private List<IWashTradeRuleBreach> _messages;

        public WashTradeCachedMessageSender(
            IWashTradeRuleMessageSender messageSender,
            ILogger<WashTradeCachedMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IWashTradeRuleBreach>();
        }

        /// <summary>
        /// Receive and cache rule breach in memory
        /// </summary>
        public void Send(IWashTradeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.LogInformation($"Wash Trade Rule Cached Message Sender received a rule breach. Returning.");
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"Wash Trade Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                _messages = _messages.Except(duplicates).ToList();

                _logger.LogInformation($"Wash Trade Rule Cached Message Sender de duplicated {_messages.Count} alerts when processing {ruleBreach.Security.Identifiers}.");

                _messages.Add(ruleBreach);
            }
        }

        /// <summary>
        /// Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush()
        {
            lock (_lock)
            {
                _logger.LogInformation($"Wash Trade Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _logger.LogInformation($"Wash Trade Rule Cached Message Sender dispatching message for {msg.Security?.Identifiers}");
                    _messageSender.Send(msg);
                }

                var count = _messages.Count;
                _messages.RemoveAll(m => true);

                return count;
            }
        }
    }
}
