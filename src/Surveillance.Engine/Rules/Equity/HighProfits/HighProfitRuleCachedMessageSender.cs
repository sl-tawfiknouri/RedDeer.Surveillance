using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Judgements.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    /// <summary>
    /// Cache and deduplicate high profit rule messages
    /// To send the messages onto the bus explicitly call Flush
    /// </summary>
    public class HighProfitRuleCachedMessageSender : IHighProfitRuleCachedMessageSender
    {
        private readonly object _lock = new object();
        private readonly IHighProfitJudgementMapper _judgementMapper;
        private readonly ILogger<HighProfitRuleCachedMessageSender> _logger;
        private List<IHighProfitJudgementContext> _messages;

        public HighProfitRuleCachedMessageSender(
            IHighProfitJudgementMapper judgementMapper,
            ILogger<HighProfitRuleCachedMessageSender> logger)
        {
            _judgementMapper = judgementMapper ?? throw new ArgumentNullException(nameof(judgementMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messages = new List<IHighProfitJudgementContext>();
        }

        /// <summary>
        /// Receive and cache rule breach in memory
        /// </summary>
        public void Send(IHighProfitJudgementContext judgementContext)
        {
            if (judgementContext == null)
            {
                return;
            }

            lock (_lock)
            {
                //_logger.LogInformation($"High Profit Rule Cached Message Sender received rule breach for {judgementContext.Security.Identifiers}");

                //var duplicates = _messages.Where(msg => msg.Trades.PositionIsSubsetOf(judgementContext.Trades)).ToList();
                //_messages = _messages.Except(duplicates).ToList();

                //_logger.LogInformation($"High Profit Rule Cached Message Sender deduplicating {_messages.Count()} for {judgementContext.Security.Identifiers}");

                //_messages.Add(judgementContext);
            }
        }

        public void Remove(ITradePosition position)
        {
            if (position == null
                || !position.Get().Any())
            {
                return;
            }

            //lock (_lock)
            //{
            //    var duplicates =
            //        _messages
            //            .Where(msg => msg != null)
            //            .Where(msg => msg.MarketClosureVirtualProfitComponent)
            //            .Where(msg => msg.Trades.PositionIsSubsetOf(position))
            //            .ToList();

            //    _messages = _messages.Except(duplicates).ToList();
           // }
        }

        /// <summary>
        /// Empty all the active cached messages across the network onto the message bus
        /// </summary>
        public int Flush()
        {
            lock (_lock)
            {
                _logger.LogInformation($"High Profit Rule Cached Message Sender dispatching {_messages.Count} rule breaches to message bus");

                //foreach (var msg in _messages)
                //{
                //    _logger.LogInformation($"High Profit Rule Cached Message Sender dispatching {msg?.Security?.Identifiers} rule breaches to message bus");
                //    _judgementMapper.Send(msg);
                //}

                var count = _messages.Count;
                _messages.RemoveAll(m => true);

                return count;
            }
        }

        public void Delete()
        {
            lock (_lock)
            {
                _logger.LogInformation($"High Profit Rule Cached Message Sender deleting alert messages");
                _messages = new List<IHighProfitJudgementContext>();
            }
        }
    }
}
