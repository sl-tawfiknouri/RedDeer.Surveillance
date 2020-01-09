namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighVolume
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The fixed income high volume tests.
    /// </summary>
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
        private IUniverseEquityMarketCacheFactory equityMarketCacheFactory;

        /// <summary>
        /// The market cache factory.
        /// </summary>
        private IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory;

        /// <summary>
        /// The parameters.
        /// </summary>
        private IHighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The rule context.
        /// </summary>
        private ISystemProcessOperationRunRuleContext ruleContext;

        /// <summary>
        /// The market trading hours service.
        /// </summary>
        private IMarketTradingHoursService marketTradingHoursService;

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
            Assert.Throws<ArgumentNullException>(
                () => new FixedIncomeHighVolumeRule(
                    this.parameters,
                    this.fixedIncomeOrderFile,
                    this.ruleContext,
                    this.equityMarketCacheFactory,
                    this.fixedIncomeMarketCacheFactory,
                    null,
                    null,
                    this.marketTradingHoursService,
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
            this.equityMarketCacheFactory = A.Fake<IUniverseEquityMarketCacheFactory>();
            this.fixedIncomeMarketCacheFactory = A.Fake<IUniverseFixedIncomeMarketCacheFactory>();
            this.marketTradingHoursService = A.Fake<IMarketTradingHoursService>();
            this.tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }
    }
}