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
        /// <summary>
        /// The ring buffer limit on rules to monitor for cancellation.
        /// </summary>
        private const int RingBufferLimit = 1000;

        /// <summary>
        /// The cancellable rules.
        /// </summary>
        private readonly HashSet<ICancellableRule> cancellableRules;

        /// <summary>
        /// The cancelled runs.
        /// </summary>
        private readonly RingBuffer<string> cancellations;

        /// <summary>
        /// The _lock on cancellation calls
        /// </summary>
        private readonly object ruleCancellationLock = new object();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IRuleCancellation> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCancellation"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RuleCancellation(ILogger<IRuleCancellation> logger)
        {
            this.cancellableRules = new HashSet<ICancellableRule>();
            this.cancellations = new RingBuffer<string>(RingBufferLimit);

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ensure this is kept idempotent
        /// </summary>
        /// <param name="cancelledRuleId">
        /// The cancelled Rule Id.
        /// </param>
        public void Cancel(string cancelledRuleId)
        {
            if (string.IsNullOrWhiteSpace(cancelledRuleId))
            {
                this.logger?.LogError("Cancel received a null cancellation id");
                return;
            }

            lock (this.ruleCancellationLock)
            {
                this.cancellations.Add(cancelledRuleId);

                var rulesToCancel = this.cancellableRules.Where(
                        _ => string.Equals(
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

                    if (!this.cancellableRules.Contains(rule))
                    {
                        continue;
                    }

                    this.cancellableRules.Remove(rule);
                }
            }
        }

        /// <summary>
        /// The subscribe functionality for cancellable rules to provide access to cancellation requests.
        /// </summary>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        public void Subscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                this.logger?.LogError("Cancel received a null cancellable rule subscription");
                return;
            }

            lock (this.ruleCancellationLock)
            {
                if (this.cancellableRules.Contains(cancellableRule))
                {
                    return;
                }

                this.cancellableRules.Add(cancellableRule);
            }

            this.PostSubscriptionCheck(cancellableRule);
        }

        /// <summary>
        /// The unsubscribe functionality for cancellable rules
        /// </summary>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        public void Unsubscribe(ICancellableRule cancellableRule)
        {
            if (cancellableRule == null)
            {
                this.logger?.LogError("Cancel received a null cancellable rule unsubscribe");
                return;
            }

            lock (this.ruleCancellationLock)
            {
                if (!this.cancellableRules.Contains(cancellableRule))
                {
                    return;
                }

                this.cancellableRules.Remove(cancellableRule);
            }
        }

        /// <summary>
        /// The post subscription check.
        /// </summary>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        private void PostSubscriptionCheck(ICancellableRule cancellableRule)
        {
            var matchedCancellations =
                this
                    .cancellations
                    .All()
                    .Where(_ => 
                        string.Equals(_, cancellableRule?.Schedule?.CorrelationId, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchedCancellations.Any())
            {
                return;
            }

            foreach (var cancelId in matchedCancellations)
            {
                this.Cancel(cancelId);
            }
        }
    }
}