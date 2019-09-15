namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class FixedIncomeHighProfitsRule : IFixedIncomeHighProfitsRule
    {
        private readonly IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters;

        private readonly ILogger<FixedIncomeHighProfitsRule> logger;

//        private readonly IHighProfitMarketClosureRule _marketClosureRule;

        private readonly IFixedIncomeHighProfitsStreamRule streamRule;

        public FixedIncomeHighProfitsRule(
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
            IFixedIncomeHighProfitsStreamRule streamRule,
  //          IHighProfitMarketClosureRule marketClosureRule,
            ILogger<FixedIncomeHighProfitsRule> logger)
        {
            this.fixedIncomeParameters =
                fixedIncomeParameters ?? throw new ArgumentNullException(nameof(fixedIncomeParameters));
            this.streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
//            this._marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public Rules Rule { get; } = Rules.HighProfits;

        public string Version { get; } = FixedIncomeHighProfitFactory.Version;

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new FixedIncomeHighProfitsRule(
                this.fixedIncomeParameters,
             (IFixedIncomeHighProfitsStreamRule)this.streamRule.Clone(factor),
         //       (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(factor),
                this.logger);
            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        public object Clone()
        {
            var cloneRule = new FixedIncomeHighProfitsRule(
                this.fixedIncomeParameters,
                (IFixedIncomeHighProfitsStreamRule)this.streamRule.Clone(),
//                (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(),
                this.logger);

            return cloneRule;
        }

        public void OnCompleted()
        {
            this.logger.LogInformation(
                "OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            this.streamRule.OnCompleted();
//            this._marketClosureRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this.logger.LogError("OnError() event received", error);
            this.streamRule.OnError(error);
 //           this._marketClosureRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            this.logger.LogInformation(
                $"OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            if (this.fixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                this.streamRule.OnNext(value);
               // this._marketClosureRule.OnNext(value);
            }

            if (this.fixedIncomeParameters.PerformHighProfitDailyAnalysis
                && !this.fixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                //this._marketClosureRule.OnNext(value);
            }
        } 
    }
}