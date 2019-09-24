// ReSharper disable AssignNullToNotNullAttribute

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    ///     Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitsRuleEquitiesParameters _equitiesParameters;

        private readonly ILogger<HighProfitsRule> _logger;

        private readonly IHighProfitMarketClosureRule _marketClosureRule;

        private readonly IHighProfitStreamRule _streamRule;

        public HighProfitsRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            IHighProfitStreamRule streamRule,
            IHighProfitMarketClosureRule marketClosureRule,
            ILogger<HighProfitsRule> logger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            this._marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public Rules Rule { get; } = Rules.HighProfits;

        public string Version { get; } = EquityRuleHighProfitFactory.Version;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new HighProfitsRule(
                this._equitiesParameters,
                (IHighProfitStreamRule)this._streamRule.Clone(factor),
                (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(factor),
                this._logger);
            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        public object Clone()
        {
            var cloneRule = new HighProfitsRule(
                this._equitiesParameters,
                (IHighProfitStreamRule)this._streamRule.Clone(),
                (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(),
                this._logger);

            return cloneRule;
        }

        public void OnCompleted()
        {
            this._logger.LogInformation(
                "OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            this._streamRule.OnCompleted();
            this._marketClosureRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._logger.LogError("OnCompleted() event received", error);
            this._streamRule.OnError(error);
            this._marketClosureRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            this._logger.LogInformation(
                $"OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            if (this._equitiesParameters.PerformHighProfitWindowAnalysis)
            {
                this._streamRule.OnNext(value);
                this._marketClosureRule.OnNext(value);
            }

            if (this._equitiesParameters.PerformHighProfitDailyAnalysis
                && !this._equitiesParameters.PerformHighProfitWindowAnalysis) this._marketClosureRule.OnNext(value);
        }
    }
}