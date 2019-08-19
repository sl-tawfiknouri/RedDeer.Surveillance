namespace Surveillance.Engine.Rules.Rules.Cancellation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Streams;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

    /// <summary>
    ///     Cancellation singleton across all threads
    /// </summary>
    public class RuleCancellation : IRuleCancellation
    {
        private const int RingBufferLimit = 1000;

        private readonly HashSet<ICancellableRule> _cancellableRules;

        private readonly RingBuffer<string> _cancellations;

        private readonly object _lock = new object();

        private readonly ILogger<IRuleCancellation> _logger;

        public RuleCancellation(ILogger<IRuleCancellation> logger)
        {
            this._cancellableRules = new HashSet<ICancellableRule>();
            this._cancellations = new RingBuffer<string>(RingBufferLimit);

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Ensure this is kept idempotent
        /// </summary>
        public void Cancel(string cancelledRuleId)
        {
            if (string.IsNullOrWhiteSpace(cancelledRuleId))
            {
                this._logger?.LogError("Cancel received a null cancellation id");
                return;
            }

            lock (this._lock)
            {
                this._cancellations.Add(cancelledRuleId);

                var rulesToCancel = this._cancellableRules.Where(
                        _ => string.Equals(
                            _.Schedule?.CorrelationId,
                            cancelledRuleId,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!rulesToCancel.Any()) return;

                foreach (var rule in rulesToCancel)
                {
                    rule.CancellationToken.Cancel();

                    if (!this._cancellableRules.Contains(rule)) return;

                    this._cancellableRules.Remove(rule);
                }
            }
        }

        public void Subscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                this._logger?.LogError("Cancel received a null cancellable rule subscription");
                return;
            }

            lock (this._lock)
            {
                if (this._cancellableRules.Contains(cancellableRule)) return;

                this._cancellableRules.Add(cancellableRule);
            }

            this._PostSubscriptionCheck(cancellableRule);
        }

        public void Unsubscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                this._logger?.LogError("Cancel received a null cancellable rule unsubscribe");
                return;
            }

            lock (this._lock)
            {
                if (!this._cancellableRules.Contains(cancellableRule)) return;

                this._cancellableRules.Remove(cancellableRule);
            }
        }

        private void _PostSubscriptionCheck(ICancellableRule cancellableRule)
        {
            var matchedCancellations = this._cancellations.All().Where(
                    _ => string.Equals(_, cancellableRule?.Schedule?.CorrelationId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchedCancellations.Any()) return;

            foreach (var cancelId in matchedCancellations) this.Cancel(cancelId);
        }
    }
}