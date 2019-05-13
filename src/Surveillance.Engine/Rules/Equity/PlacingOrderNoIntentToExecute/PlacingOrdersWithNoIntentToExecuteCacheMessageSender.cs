using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    public class PlacingOrdersWithNoIntentToExecuteCacheMessageSender : IPlacingOrdersWithNoIntentToExecuteCacheMessageSender
    {
        private readonly object _lock = new object();
        private readonly IPlacingOrdersWithNoIntentToExecuteMessageSender _messageSender;
        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteCacheMessageSender> _logger;
        private List<IPlacingOrdersWithNoIntentToExecuteRuleBreach> _messages;

        public PlacingOrdersWithNoIntentToExecuteCacheMessageSender(
            IPlacingOrdersWithNoIntentToExecuteMessageSender messageSender,
            ILogger<PlacingOrdersWithNoIntentToExecuteCacheMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IPlacingOrdersWithNoIntentToExecuteRuleBreach>();
        }

        public void Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.LogInformation($"received a null rule breach. Returning.");
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                _messages = _messages.Except(duplicates).ToList();
                _messages.Add(ruleBreach);
            }
        }

        public int Flush()
        {
            lock (_lock)
            {
                _logger.LogInformation($"dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _logger.LogInformation($"dispatching {msg.Security.Identifiers} rule breach to message bus");
                    _messageSender.Send(msg);
                }

                var count = _messages.Count;
                _messages.RemoveAll(m => true);

                return count;
            }
        }
    }
}
