using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Rules.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Engine.Rules.Rules.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IHighProfitMarketClosureRule _marketClosureRule;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitsRule(
            IHighProfitStreamRule streamRule,
            IHighProfitMarketClosureRule marketClosureRule,
            ILogger<HighProfitsRule> logger)
        {
            _streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            _marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public void OnCompleted()
        {
            _logger.LogInformation($"OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            _streamRule.OnCompleted();
            _marketClosureRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"OnCompleted() event received", error);
            _streamRule.OnError(error);
            _marketClosureRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _logger.LogInformation($"OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            _streamRule.OnNext(value);
            _marketClosureRule.OnNext(value);
        }

        public Domain.Scheduling.Rules Rule { get; } = Domain.Scheduling.Rules.HighProfits;
        public string Version { get; } = HighProfitRuleFactory.Version;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new HighProfitsRule(
                (IHighProfitStreamRule)_streamRule.Clone(factor),
                (IHighProfitMarketClosureRule)_marketClosureRule.Clone(factor),
                _logger);
            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        public object Clone()
        {
            var cloneRule = new HighProfitsRule(
                (IHighProfitStreamRule)_streamRule.Clone(),
                (IHighProfitMarketClosureRule)_marketClosureRule.Clone(),
                _logger);

            return cloneRule;
        }
    }
}