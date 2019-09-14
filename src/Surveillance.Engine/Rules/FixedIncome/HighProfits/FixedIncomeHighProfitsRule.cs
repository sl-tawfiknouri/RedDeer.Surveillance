namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class FixedIncomeHighProfitsRule : IFixedIncomeHighProfitsRule
    {
        private readonly IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters;

        private readonly ILogger<FixedIncomeHighProfitsRule> logger;

//        private readonly IHighProfitMarketClosureRule _marketClosureRule;

  //      private readonly IHighProfitStreamRule _streamRule;

        public FixedIncomeHighProfitsRule(
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
//            IHighProfitStreamRule streamRule,
  //          IHighProfitMarketClosureRule marketClosureRule,
            ILogger<FixedIncomeHighProfitsRule> logger)
        {
            this.fixedIncomeParameters =
                fixedIncomeParameters ?? throw new ArgumentNullException(nameof(fixedIncomeParameters));
//            this._streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
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
            //    (IHighProfitStreamRule)this._streamRule.Clone(factor),
         //       (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(factor),
                this.logger);
            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        public object Clone()
        {
            var cloneRule = new FixedIncomeHighProfitsRule(
                this.fixedIncomeParameters,
//                (IHighProfitStreamRule)this._streamRule.Clone(),
//                (IHighProfitMarketClosureRule)this._marketClosureRule.Clone(),
                this.logger);

            return cloneRule;
        }

        public void OnCompleted()
        {
            this.logger.LogInformation(
                "OnCompleted() event received. Passing onto high profit and high profit market close rules.");
//            this._streamRule.OnCompleted();
//            this._marketClosureRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this.logger.LogError("OnError() event received", error);
 //           this._streamRule.OnError(error);
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
                //this._streamRule.OnNext(value);
               // this._marketClosureRule.OnNext(value);
            }

            if (this.fixedIncomeParameters.PerformHighProfitDailyAnalysis
                && !this.fixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                //this._marketClosureRule.OnNext(value);
            }
        } 
    }


        //private readonly IUniverseAlertStream _alertStream;

        //private readonly ILogger<FixedIncomeHighProfitsRule> logger;

        //private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;

        //private readonly IHighProfitsRuleFixedIncomeParameters _parameters;

        //public FixedIncomeHighProfitsRule(
        //    IHighProfitsRuleFixedIncomeParameters parameters,
        //    IUniverseFixedIncomeOrderFilterService orderFilterService,
        //    ISystemProcessOperationRunRuleContext ruleCtx,
        //    IUniverseMarketCacheFactory factory,
        //    RuleRunMode runMode,
        //    IUniverseAlertStream alertStream,
        //    ILogger<FixedIncomeHighProfitsRule> logger,
        //    ILogger<TradingHistoryStack> tradingStackLogger)
        //    : base(
        //        parameters?.Windows.BackwardWindowSize ?? TimeSpan.FromDays(1),
        //        parameters?.Windows.BackwardWindowSize ?? TimeSpan.FromDays(1),
        //        parameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
        //        Rules.FixedIncomeHighProfits,
        //        Versioner.Version(1, 0),
        //        "Fixed Income High Profits Rule",
        //        ruleCtx,
        //        factory,
        //        runMode,
        //        logger,
        //        tradingStackLogger)
        //{
        //    this._parameters = parameters ?? throw new ArgumentNullException(nameof(this._parameters));
        //    this._orderFilterService =
        //        orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
        //    this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
        //    this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //}

        //public IFactorValue OrganisationFactorValue { get; set; }

        //public object Clone()
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Clone called at {this.UniverseDateTime}");

        //    var clone = (FixedIncomeHighProfitsRule)this.MemberwiseClone();
        //    clone.BaseClone();

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Clone completed for {this.UniverseDateTime}");
        //    return clone;
        //}

        //public IUniverseCloneableRule Clone(IFactorValue factor)
        //{
        //    var clone = (FixedIncomeHighProfitsRule)this.Clone();
        //    clone.OrganisationFactorValue = factor;

        //    return clone;
        //}

        //public override void RunOrderFilledEvent(ITradingHistoryStack history)
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunOrderFilledEvent called at {this.UniverseDateTime}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunOrderFilledEvent completed for {this.UniverseDateTime}");
        //}

        //public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        //{
        //    // do nothing
        //}

        //protected override void EndOfUniverse()
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Eschaton called at {this.UniverseDateTime}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Eschaton completed for {this.UniverseDateTime}");
        //}

        //protected override IUniverseEvent Filter(IUniverseEvent value)
        //{
        //    return this._orderFilterService.Filter(value);
        //}

        //protected override void Genesis()
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Universe Genesis called at {this.UniverseDateTime}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Universe Genesis completed for {this.UniverseDateTime}");
        //}

        //protected override void MarketClose(MarketOpenClose exchange)
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Market Close called at {this.UniverseDateTime} for {exchange?.MarketId}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Market Close completed at {this.UniverseDateTime} for {exchange?.MarketId}");
        //}

        //protected override void MarketOpen(MarketOpenClose exchange)
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Market Open called at {this.UniverseDateTime} for {exchange?.MarketId}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} Market Open completed at {this.UniverseDateTime} for {exchange?.MarketId}");
        //}

        //protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule called at {this.UniverseDateTime}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        //}

        //protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        //{
        //    // do nothing
        //}

        //protected override void RunPostOrderEvent(ITradingHistoryStack history)
        //{
        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunRule called at {this.UniverseDateTime}");

        //    this._logger.LogInformation(
        //        $"{nameof(FixedIncomeHighProfitsRule)} RunRule completed for {this.UniverseDateTime}");
        //}

        //protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        //{
        //    // do nothing
        //}
}