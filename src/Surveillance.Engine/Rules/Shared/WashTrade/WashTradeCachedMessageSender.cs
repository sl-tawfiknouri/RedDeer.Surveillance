namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;

    public class WashTradeCachedMessageSender : IWashTradeCachedMessageSender
    {
        private readonly object _lock = new object();

        private readonly ILogger<WashTradeCachedMessageSender> _logger;

        private readonly IWashTradeRuleMessageSender _messageSender;

        private List<IWashTradeRuleBreach> _messages;

        public WashTradeCachedMessageSender(
            IWashTradeRuleMessageSender messageSender,
            ILogger<WashTradeCachedMessageSender> logger)
        {
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messages = new List<IWashTradeRuleBreach>();
        }

        /// <summary>
        ///     Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush()
        {
            lock (this._lock)
            {
                this._logger.LogInformation($"dispatching {this._messages.Count} rule breaches to message bus");

                foreach (var msg in this._messages)
                {
                    this._logger.LogInformation($"dispatching message for {msg.Security?.Identifiers}");
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
        public void Send(IWashTradeRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                this._logger.LogInformation("received a rule breach. Returning.");
                return;
            }

            lock (this._lock)
            {
                this._logger.LogInformation($"received rule breach for {ruleBreach.Security.Identifiers}");

                var duplicates = this._messages.Where(msg => msg.Trades.PositionIsSubsetOf(ruleBreach.Trades)).ToList();
                this._messages = this._messages.Except(duplicates).ToList();

                this._logger.LogInformation(
                    $"deduplicated {this._messages.Count} alerts when processing {ruleBreach.Security.Identifiers}.");

                this._messages.Add(ruleBreach);
            }
        }
    }
}