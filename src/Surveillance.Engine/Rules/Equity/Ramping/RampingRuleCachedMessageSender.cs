using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingRuleCachedMessageSender : IRampingRuleCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly IRampingRuleMessageSender _messageSender;
        private readonly ILogger<IRampingRuleCachedMessageSender> _logger;
        private List<IRampingRuleBreach> _messages;

        public RampingRuleCachedMessageSender(
            IRampingRuleMessageSender messageSender,
            ILogger<IRampingRuleCachedMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IRampingRuleBreach>();
        }

        public void Send(IRampingRuleBreach breach)
        {
            if (breach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.LogInformation($"received a null rule breach. Returning.");
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"received rule breach for {breach.Security.Identifiers}");

                var duplicates = _messages.Where(_ => _.Trades.PositionIsSubsetOf(breach.Trades)).ToList();
                _messages = _messages.Except(duplicates).ToList();
                _messages.Add(breach);
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
                    _messageSender.Send(msg).Wait();
                }

                var count = _messages.Count;
                _messages.RemoveAll(_ => true);

                return count;
            }
        }
    }
}
