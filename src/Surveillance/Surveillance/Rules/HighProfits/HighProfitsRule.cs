using System;
using Microsoft.Extensions.Logging;
using Surveillance.Factories;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Rules.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IMarketCloseMultiverseTransformer _multiverseTransformer;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitsRule(
            IHighProfitStreamRule streamRule,
            IMarketCloseMultiverseTransformer multiverseTransformer,
            ILogger<HighProfitsRule> logger)
        {
            _streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            _multiverseTransformer =
                multiverseTransformer
                ?? throw new ArgumentNullException(nameof(multiverseTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"HighProfitsRule OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            _streamRule.OnCompleted();
            _multiverseTransformer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"HighProfitsRule OnCompleted() event received", error);
            _streamRule.OnError(error);
            _multiverseTransformer.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _logger.LogInformation($"HighProfitsRule OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market cap (multiverse transformer) rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            _streamRule.OnNext(value);
            _multiverseTransformer.OnNext(value);
        }

        public DomainV2.Scheduling.Rules Rule { get; } = DomainV2.Scheduling.Rules.HighProfits;
        public string Version { get; } = HighProfitRuleFactory.Version;

        public object Clone()
        {
            return new HighProfitsRule(
                (IHighProfitStreamRule)_streamRule.Clone(),
                (IMarketCloseMultiverseTransformer)_multiverseTransformer.Clone(),
                _logger);
        }
    }
}