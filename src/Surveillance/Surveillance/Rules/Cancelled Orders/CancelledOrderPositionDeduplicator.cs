using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Domain.Equity.Interfaces;
using Surveillance.Configuration.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    /// <summary>
    /// This deduplicator works in memory meaning alerts can be lost if shut down of service follows too soon during processing
    /// the loss is only possible if the lead on the frame being processed is less than the delay period
    /// </summary>
    public class CancelledOrderPositionDeDuplicator : ICancelledOrderPositionDeDuplicator
    {
        private readonly ICancelledOrderMessageSender _messageSender;
        private const int DefaultDedupeDelaySeconds = 5;
        private List<CancelledOrderMessageSenderParameters> _cancelledOrders;
        private readonly Dictionary<ISecurityIdentifiers, List<TimerParameterPair>> _securityTimers;
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
            _securityTimers = new Dictionary<ISecurityIdentifiers, List<TimerParameterPair>>();

            _timespan = TimeSpan.FromSeconds(
                ruleConfiguration
                    .CancelledOrderDeduplicationDelaySeconds
                    .GetValueOrDefault(DefaultDedupeDelaySeconds));
        }

        public void Send(CancelledOrderMessageSenderParameters parameters)
        {
            if (parameters?.TradePosition == null)
            {
                return;
            }

            lock (_lock)
            {
                var ordersForSecurityAndSubsetOfLatestPositionAlerts =
                    _cancelledOrders
                        .Where(co => Equals(co.Identifiers, parameters.Identifiers))
                        .Where(x => x.TradePosition != null && x.TradePosition.PositionIsSubsetOf(parameters.TradePosition));

                _cancelledOrders =
                    _cancelledOrders
                        .Except(ordersForSecurityAndSubsetOfLatestPositionAlerts)
                        .ToList();

                _cancelledOrders.Add(parameters);

                if (!_securityTimers.ContainsKey(parameters.Identifiers))
                {
                    SetInitialTimerPair(parameters);
                }
                else
                {
                    UpdateTimerPairs(parameters);
                }
            }
        }

        private void SetInitialTimerPair(CancelledOrderMessageSenderParameters parameters)
        {
            var timer = new Timer(_timespan.TotalMilliseconds);
            var pair = new TimerParameterPair { Timer = timer, Parameters = parameters };
            _securityTimers.Add(parameters.Identifiers, new List<TimerParameterPair> { pair });
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += OnElapse;
        }

        private void UpdateTimerPairs(CancelledOrderMessageSenderParameters parameters)
        {
            _securityTimers.TryGetValue(parameters.Identifiers, out var timerParamPairs);

            if (timerParamPairs == null)
            {
                throw new InvalidOperationException(nameof(timerParamPairs));
            }

            var paramsToUpdate =
                timerParamPairs
                    .Where(tpp => tpp.Parameters.TradePosition.PositionIsSubsetOf(parameters.TradePosition))
                    .ToList();

            if (!paramsToUpdate.Any())
            {
                var timer = new Timer(_timespan.TotalMilliseconds);
                var pair = new TimerParameterPair { Timer = timer, Parameters = parameters };
                timer.AutoReset = false;
                timer.Enabled = true;
                timer.Elapsed += OnElapse;
                timerParamPairs.Add(pair);
            }
            else
            {
                foreach (var item in paramsToUpdate)
                {
                    item.Parameters = parameters;
                }
            }
        }

        private void OnElapse(object sender, EventArgs e)
        {
            lock (_lock)
            {
               var identifiersForTimer = _securityTimers.FirstOrDefault(st => st.Value.Any(coll => coll.Timer == sender));

                if (identifiersForTimer.Key == null)
                {
                    return;
                }

                var paramsToSend = identifiersForTimer.Value.Where(ift => ift.Timer == sender).ToList();

                foreach (var item in paramsToSend)
                {
                    _Send(item.Parameters);
                    item.Timer.Dispose();
                    identifiersForTimer.Value.Remove(item);
                }
            }
        }

        private void _Send(CancelledOrderMessageSenderParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _messageSender.Send(parameters.RuleBreach);
        }

        private class TimerParameterPair
        {
            public Timer Timer { get; set; }
            public CancelledOrderMessageSenderParameters Parameters { get; set; }
        }
    }
}