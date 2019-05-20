using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Streams;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    /// <summary>
    /// Cancellation singleton across all threads
    /// </summary>
    public class RuleCancellation : IRuleCancellation
    {
        private readonly object _lock = new object();
        private readonly RingBuffer<string> _cancellations;
        private const int RingBufferLimit = 10;

        private readonly ILogger<IRuleCancellation> _logger;
        private readonly HashSet<ICancellableRule> _cancellableRules;

        public RuleCancellation(ILogger<IRuleCancellation> logger)
        {
            _cancellableRules = new HashSet<ICancellableRule>();
            _cancellations = new RingBuffer<string>(RingBufferLimit);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ensure this is kept idempotent
        /// </summary>
        public void Cancel(string cancelledRuleId)
        {
            if (string.IsNullOrWhiteSpace(cancelledRuleId))
            {
                _logger?.LogError($"Cancel received a null cancellation id");
                return;
            }

            lock (_lock)
            {
                _cancellations.Add(cancelledRuleId);

                var rulesToCancel =
                    _cancellableRules
                        .Where(_ =>
                            string.Equals(
                                _.Schedule?.CorrelationId,
                                cancelledRuleId,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (!rulesToCancel.Any())
                {
                    return;
                }

                foreach (var rule in rulesToCancel)
                {
                    rule.CancellationToken.Cancel();

                    if (!_cancellableRules.Contains(rule))
                    {
                        return;
                    }

                    _cancellableRules.Remove(rule);
                }
            }
        }

        public void Subscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                _logger?.LogError($"Cancel received a null cancellable rule subscription");
                return;
            }

            lock (_lock)
            {
                if (_cancellableRules.Contains(cancellableRule))
                {
                    return;
                }

                _cancellableRules.Add(cancellableRule);
            }

            _PostSubscriptionCheck(cancellableRule);
        }

        private void _PostSubscriptionCheck(ICancellableRule cancellableRule)
        {
            var matchedCancellations = _cancellations
                .All()
                .Where(_ =>
                    string.Equals(
                        _,
                        cancellableRule?.Schedule?.CorrelationId,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchedCancellations.Any())
            {
                return;
            }

            foreach (var cancelId in matchedCancellations)
            {
                Cancel(cancelId);
            }
        }

        public void Unsubscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                _logger?.LogError($"Cancel received a null cancellable rule unsubscribe");
                return;
            }

            lock (_lock)
            {
                if (!_cancellableRules.Contains(cancellableRule))
                {
                    return;
                }

                _cancellableRules.Remove(cancellableRule);
            }
        }
    }
}
