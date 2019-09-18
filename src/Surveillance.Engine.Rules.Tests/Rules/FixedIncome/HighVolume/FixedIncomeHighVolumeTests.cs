namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighVolume
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class FixedIncomeHighVolumeTests
    {
        /// <summary>
        /// The _run mode.
        /// </summary>
        private readonly RuleRunMode _runMode = RuleRunMode.ForceRun;

        /// <summary>
        /// The fixed income order file.
        /// </summary>
        private IUniverseFixedIncomeOrderFilterService fixedIncomeOrderFile;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private IUniverseMarketCacheFactory marketCacheFactory;

        /// <summary>
        /// The parameters.
        /// </summary>
        private IHighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementService judgementService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<FixedIncomeHighVolumeRule> logger;

        /// <summary>
        /// The trading stack logger.
        /// </summary>
        private ILogger<TradingHistoryStack> tradingStackLogger;

        /// <summary>
        /// The constructor logger null throws exception.
        /// </summary>
        [Test]
        public void ConstructorLoggerNullThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FixedIncomeHighVolumeRule(
                    this.parameters,
                    this.fixedIncomeOrderFile,
                    this.ruleContext,
                    this.marketCacheFactory,
                    this.judgementService,
                    this.dataRequestSubscriber,
                    this._runMode,
                    null,
                    this.tradingStackLogger));
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.parameters = A.Fake<IHighVolumeIssuanceRuleFixedIncomeParameters>();
            this.fixedIncomeOrderFile = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this.ruleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this.marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this.logger = new NullLogger<FixedIncomeHighVolumeRule>();
            this.tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }
    }
}