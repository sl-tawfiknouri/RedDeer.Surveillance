using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Domain.Equity.Interfaces;
using Surveillance.Configuration.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderPositionDeDuplicator
    {
        private readonly ICancelledOrderMessageSender _messageSender;
        private const int DefaultDedupeDelaySeconds = 5;
        private List<CancelledOrderMessageSenderParameters> _cancelledOrders;
        private readonly Dictionary<ISecurityIdentifiers, Timer> _securityTimers;
        private readonly TimeSpan _timespan;

        private readonly object _lock = new object();

        public CancelledOrderPositionDeDuplicator(
            IRuleConfiguration ruleConfiguration,
            ICancelledOrderMessageSender messageSender)
        {
            if (ruleConfiguration == null)
            {
                throw new ArgumentNullException(nameof(ruleConfiguration));
            }

            _cancelledOrders = new List<CancelledOrderMessageSenderParameters>();
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _securityTimers = new Dictionary<ISecurityIdentifiers, Timer>();

            _timespan = TimeSpan.FromSeconds(
                ruleConfiguration
                    .CancelledOrderDeduplicationDelaySeconds
                    .GetValueOrDefault(DefaultDedupeDelaySeconds));
        }

        public void Send(CancelledOrderMessageSenderParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            lock (_lock)
            {
                var equalByIdentifiers = _cancelledOrders.Where(co => Equals(co.Identifiers, parameters.Identifiers));
                _cancelledOrders = _cancelledOrders.Except(equalByIdentifiers).ToList();
                _cancelledOrders.Add(parameters);

                if (!_securityTimers.ContainsKey(parameters.Identifiers))
                {
                    var timer = new Timer(_timespan.TotalMilliseconds);
                    _securityTimers.Add(parameters.Identifiers, timer);
                    timer.AutoReset = false;
                    timer.Enabled = true;
                    timer.Elapsed += OnElapse;
                }
            }
        }

        private void OnElapse(object sender, EventArgs e)
        {
            lock (_lock)
            {
                var identifiersForTimer = _securityTimers.FirstOrDefault((x) => x.Value == sender);
                if (identifiersForTimer.Key == null)
                {
                    return;
                }

                var itemsToSendOn =
                    _cancelledOrders
                        .Where(co => Equals(co.Identifiers, identifiersForTimer.Key))
                        .ToList();

                foreach (var item in itemsToSendOn)
                {
                    _cancelledOrders.Remove(item);
                    _Send(item);
                }

                _securityTimers.Remove(identifiersForTimer.Key);
            }
        }

        private void _Send(CancelledOrderMessageSenderParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _messageSender.Send(parameters.TradePosition, parameters.RuleBreach, parameters.Parameters);
        }
    }
}