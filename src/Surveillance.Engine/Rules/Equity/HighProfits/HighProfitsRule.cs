using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitsRuleEquitiesParameters _equitiesParameters;
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IHighProfitMarketClosureRule _marketClosureRule;
        private readonly ILogger<HighProfitsRule> _logger;

        public HighProfitsRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            IHighProfitStreamRule streamRule,
            IHighProfitMarketClosureRule marketClosureRule,
            ILogger<HighProfitsRule> logger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
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
            if (_equitiesParameters.PerformHighProfitWindowAnalysis)
            {
                _streamRule.OnNext(value);
                _marketClosureRule.OnNext(value);
            }

            if (_equitiesParameters.PerformHighProfitDailyAnalysis
                && !_equitiesParameters.PerformHighProfitWindowAnalysis)
            {
                _marketClosureRule.OnNext(value);
            }
        }

        public Domain.Surveillance.Scheduling.Rules Rule { get; } = Domain.Surveillance.Scheduling.Rules.HighProfits;
        public string Version { get; } = EquityRuleHighProfitFactory.Version;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new HighProfitsRule(
                _equitiesParameters,
                (IHighProfitStreamRule)_streamRule.Clone(factor),
                (IHighProfitMarketClosureRule)_marketClosureRule.Clone(factor),
                _logger);
            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        public object Clone()
        {
            var cloneRule = new HighProfitsRule(
                _equitiesParameters,
                (IHighProfitStreamRule)_streamRule.Clone(),
                (IHighProfitMarketClosureRule)_marketClosureRule.Clone(),
                _logger);

            return cloneRule;
        }
    }
}