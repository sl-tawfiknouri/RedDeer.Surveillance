namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;

    public class CancelledOrderRuleCachedMessageSender : ICancelledOrderRuleCachedMessageSender
    {
        private readonly object _lock = new object();

        private readonly ILogger<CancelledOrderRuleCachedMessageSender> _logger;

        private readonly ICancelledOrderMessageSender _messageSender;

        private List<ICancelledOrderRuleBreach> _messages;

        public CancelledOrderRuleCachedMessageSender(
            ICancelledOrderMessageSender messageSender,
            ILogger<CancelledOrderRuleCachedMessageSender> logger)
        {
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messages = new List<ICancelledOrderRuleBreach>();
        }

        /// <summary>
        ///     Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush()
        {
            lock (this._lock)
            {
                this._logger.LogInformation(
                    $"Cancelled Order Rule Cached Message Sender dispatching {this._messages.Count} rule breaches to message bus");

                foreach (var msg in this._messages)
                {
                    this._logger.LogInformation(
                        $"Cancelled Order Rule Cached Message Sender sending message {msg.Security?.Identifiers} to message bus");
                    this._messageSender.Send(msg);
                }

                var count = this._messages.Count;
                this._messages.RemoveAll(m => true);
                this._logger.LogInformation(
                    $"Cancelled Order Rule Cached Message Sender dispatched {this._messages.Count} rule breaches to message bus");

                return count;
            }
        }

        /// <summary>
        ///     Receive and cache rule breach in memory
        /// </summary>
        public void Send(ICancelledOrderRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                this._logger.LogInformation(
                    $"Cancelled Order Rule Cached Message Sender received a null rule breach {ruleBreach.Security.Identifiers}. Returning.");
                return;
            }

            lock (this._lock)
            {
                this._logger.LogInformation(
                    $"Cancelled Order Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = this._messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();

                if (duplicates != null && duplicates.Any())
                    this._logger.LogInformation(
                        $"Cancelled Order Rule Cached Message Sender removing {duplicates.Count} duplicates for  {ruleBreach.Security.Identifiers}");

                this._messages = this._messages.Except(duplicates).ToList();
                this._messages.Add(ruleBreach);
            }
        }
    }
}