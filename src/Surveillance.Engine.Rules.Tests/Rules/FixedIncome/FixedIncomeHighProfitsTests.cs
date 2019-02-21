using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome
{
    [TestFixture]
    public class FixedIncomeHighProfitsTests
    {
        private RuleRunMode _runMode = RuleRunMode.ForceRun;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseMarketCacheFactory _marketCacheFactory;
        private ILogger<FixedIncomeHighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingStackLogger;

        [SetUp]
        public void Setup()
        {
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<FixedIncomeHighProfitsRule>();
            _tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new FixedIncomeHighProfitsRule(
                    TimeSpan.Zero,
                    _ruleCtx,
                    _marketCacheFactory,
                    _runMode,
                    null,
                    _tradingStackLogger));
        }
    }
}
