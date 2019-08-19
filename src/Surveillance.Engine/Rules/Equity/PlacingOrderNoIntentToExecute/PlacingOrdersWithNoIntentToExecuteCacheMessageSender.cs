namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;

    public class
        PlacingOrdersWithNoIntentToExecuteCacheMessageSender : IPlacingOrdersWithNoIntentToExecuteCacheMessageSender
    {
        private readonly object _lock = new object();

        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteCacheMessageSender> _logger;

        private readonly IPlacingOrdersWithNoIntentToExecuteMessageSender _messageSender;

        private List<IPlacingOrdersWithNoIntentToExecuteRuleBreach> _messages;

        public PlacingOrdersWithNoIntentToExecuteCacheMessageSender(
            IPlacingOrdersWithNoIntentToExecuteMessageSender messageSender,
            ILogger<PlacingOrdersWithNoIntentToExecuteCacheMessageSender> logger)
        {
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messages = new List<IPlacingOrdersWithNoIntentToExecuteRuleBreach>();
        }

        public int Flush()
        {
            lock (this._lock)
            {
                this._logger.LogInformation($"dispatching {this._messages.Count} rule breaches to message bus");

                foreach (var msg in this._messages)
                {
                    this._logger.LogInformation($"dispatching {msg.Security.Identifiers} rule breach to message bus");
                    this._messageSender.Send(msg);
                }

                var count = this._messages.Count;
                this._messages.RemoveAll(m => true);

                return count;
            }
        }

        public void Send(IPlacingOrdersWithNoIntentToExecuteRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                this._logger.LogInformation("received a null rule breach. Returning.");
                return;
            }

            lock (this._lock)
            {
                this._logger.LogInformation($"received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = this._messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                this._messages = this._messages.Except(duplicates).ToList();
                this._messages.Add(ruleBreach);
            }
        }
    }
}