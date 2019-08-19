namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighVolume
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class FixedIncomeHighVolumeTests
    {
        private readonly RuleRunMode _runMode = RuleRunMode.ForceRun;

        private IUniverseAlertStream _alertStream;

        private IUniverseFixedIncomeOrderFilterService _fixedIncomeOrderFile;

        private ILogger<FixedIncomeHighProfitsRule> _logger;

        private IUniverseMarketCacheFactory _marketCacheFactory;

        private IHighVolumeIssuanceRuleFixedIncomeParameters _parameters;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private ILogger<TradingHistoryStack> _tradingStackLogger;

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FixedIncomeHighVolumeIssuanceRule(
                    this._parameters,
                    this._fixedIncomeOrderFile,
                    this._ruleCtx,
                    this._marketCacheFactory,
                    this._runMode,
                    this._alertStream,
                    null,
                    this._tradingStackLogger));
        }

        [SetUp]
        public void Setup()
        {
            this._parameters = A.Fake<IHighVolumeIssuanceRuleFixedIncomeParameters>();
            this._fixedIncomeOrderFile = A.Fake<IUniverseFixedIncomeOrderFilterService>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._logger = new NullLogger<FixedIncomeHighProfitsRule>();
            this._tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }
    }
}