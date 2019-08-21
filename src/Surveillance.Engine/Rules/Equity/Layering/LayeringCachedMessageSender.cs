namespace Surveillance.Engine.Rules.Rules.Equity.Layering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;

    public class LayeringCachedMessageSender : ILayeringCachedMessageSender
    {
        private readonly object _lock = new object();

        private readonly ILogger<LayeringCachedMessageSender> _logger;

        private readonly ILayeringAlertSender _messageSender;

        private List<ILayeringRuleBreach> _messages;

        public LayeringCachedMessageSender(
            ILayeringAlertSender messageSender,
            ILogger<LayeringCachedMessageSender> logger)
        {
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messages = new List<ILayeringRuleBreach>();
        }

        /// <summary>
        ///     Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush()
        {
            lock (this._lock)
            {
                this._logger.LogInformation(
                    $"Layering Rule Cached Message Sender dispatching {this._messages.Count} rule breaches to message bus");

                foreach (var msg in this._messages)
                {
                    this._logger.LogInformation(
                        $"Layering Rule Cached Message Sender dispatching {msg.Security.Identifiers} rule breach to message bus");
                    this._messageSender.Send(msg);
                }

                var count = this._messages.Count;
                this._messages.RemoveAll(m => true);

                return count;
            }
        }

        /// <summary>
        ///     Receive and cache rule breach in memory
        /// </summary>
        public void Send(ILayeringRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                this._logger.LogInformation(
                    "Layering Rule Cached Message Sender received a null rule breach. Returning.");
                return;
            }

            lock (this._lock)
            {
                this._logger.LogInformation(
                    $"Layering Rule Cached Message Sender received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = this._messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                this._messages = this._messages.Except(duplicates).ToList();
                this._messages.Add(ruleBreach);
            }
        }
    }
}