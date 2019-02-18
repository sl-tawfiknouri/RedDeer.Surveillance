using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Rules.HighVolume.Interfaces;

namespace Surveillance.Engine.Rules.Rules.HighVolume
{
    public class HighVolumeRuleCachedMessageSender : IHighVolumeRuleCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly IHighVolumeMessageSender _messageSender;
        private readonly ILogger<HighVolumeRuleCachedMessageSender> _logger;
        private List<IHighVolumeRuleBreach> _messages;

        public HighVolumeRuleCachedMessageSender(
            IHighVolumeMessageSender messageSender,
            ILogger<HighVolumeRuleCachedMessageSender> logger)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IHighVolumeRuleBreach>();
        }

        /// <summary>
        /// Receive and cache rule breach in memory
        /// </summary>
        public void Send(IHighVolumeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.LogInformation($"High Volume Rule Cached Message Sender received a rule breach that was null. Returning.");
                return;
            }

            lock (_lock)
            {
                _logger.LogInformation($"High Volume Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
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
                _logger.LogInformation($"High Volume Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                foreach (var msg in _messages)
                {
                    _logger.LogInformation($"High Volume Rule Cached Message Sender dispatching {msg.Security?.Identifiers} rule breach to message bus");
                    _messageSender.Send(msg);
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
                _logger.LogInformation($"HighVolumeRuleCachedMessageSender deleting alerts");
                _messages = new List<IHighVolumeRuleBreach>();
            }
        }
    }
}
