namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

    public class RampingRuleCachedMessageSender : IRampingRuleCachedMessageSender
    {
        private readonly object _lock = new object();

        private readonly ILogger<IRampingRuleCachedMessageSender> _logger;

        private readonly IRampingRuleMessageSender _messageSender;

        private List<IRampingRuleBreach> _messages;

        public RampingRuleCachedMessageSender(
            IRampingRuleMessageSender messageSender,
            ILogger<IRampingRuleCachedMessageSender> logger)
        {
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messages = new List<IRampingRuleBreach>();
        }

        public int Flush()
        {
            lock (this._lock)
            {
                this._logger.LogInformation($"dispatching {this._messages.Count} rule breaches to message bus");

                foreach (var msg in this._messages)
                {
                    this._logger.LogInformation($"dispatching {msg.Security.Identifiers} rule breach to message bus");
                    this._messageSender.Send(msg).Wait();
                }

                var count = this._messages.Count;
                this._messages.RemoveAll(_ => true);

                return count;
            }
        }

        public void Send(IRampingRuleBreach breach)
        {
            if (breach == null)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                this._logger.LogInformation("received a null rule breach. Returning.");
                return;
            }

            lock (this._lock)
            {
                this._logger.LogInformation($"received rule breach for {breach.Security.Identifiers}");

                var duplicates = this._messages.Where(_ => _.Trades.PositionIsSubsetOf(breach.Trades)).ToList();
                this._messages = this._messages.Except(duplicates).ToList();
                this._messages.Add(breach);
            }
        }
    }
}