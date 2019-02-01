using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Rules.CancelledOrders
{
    public class CancelledOrderRuleCachedMessageSender : ICancelledOrderRuleCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly ICancelledOrderMessageSender _messageSender;
        private readonly ILogger<CancelledOrderRuleCachedMessageSender> _logger;
        private List<ICancelledOrderRuleBreach> _messages;

        public CancelledOrderRuleCachedMessageSender(
            ICancelledOrderMessageSender messageSender,
            ILogger<CancelledOrderRuleCachedMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<ICancelledOrderRuleBreach>();
        }

        /// <summary>
        /// Receive and cache rule breach in memory
        /// </summary>
        public void Send(ICancelledOrderRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender received a null rule breach {ruleBreach.Security.Identifiers}. Returning.");
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();

                if (duplicates?.Any() ?? false)
                {
                    _logger.LogInformation($"Cancelled Order Rule Cached Message Sender removing {duplicates.Count} duplicates for  {ruleBreach.Security.Identifiers}");
                }

                _messages = _messages.Except(duplicates).ToList();
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
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _logger.LogInformation($"Cancelled Order Rule Cached Message Sender sending message {msg.Security?.Identifiers} to message bus");
                    _messageSender.Send(msg);
                }

                var count = _messages.Count;
                _messages.RemoveAll(m => true);
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender dispatched {_messages.Count} rule breaches to message bus");

                return count;
            }
        }
    }
}
