using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

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
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                _messages = _messages.Except(duplicates).ToList();
                _messages.Add(ruleBreach);
            }
        }

        /// <summary>
        /// Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush(ISystemProcessOperationRunRuleContext opCtx)
        {
            lock (_lock)
            {
                _logger.LogInformation($"Cancelled Order Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _messageSender.Send(msg, opCtx);
                }

                var count = _messages.Count;
                _messages.RemoveAll(m => true);

                return count;
            }
        }
    }
}
