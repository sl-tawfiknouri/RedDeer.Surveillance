﻿using System;
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
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.HighProfits
{
    [TestFixture]
    public class FixedIncomeHighProfitsTests
    {
        private RuleRunMode _runMode = RuleRunMode.ForceRun;
        private IHighProfitsRuleFixedIncomeParameters _parameters;
        private IUniverseFixedIncomeOrderFilter _fixedIncomeOrderFile;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseMarketCacheFactory _marketCacheFactory;
        private IUniverseAlertStream _alertStream;
        private ILogger<FixedIncomeHighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingStackLogger;

        [SetUp]
        public void Setup()
        {
            _fixedIncomeOrderFile = A.Fake<IUniverseFixedIncomeOrderFilter>();
            _parameters = A.Fake<IHighProfitsRuleFixedIncomeParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _logger = new NullLogger<FixedIncomeHighProfitsRule>();
            _tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new FixedIncomeHighProfitsRule(
                    _parameters,
                    _fixedIncomeOrderFile,
                    _ruleCtx,
                    _marketCacheFactory,
                    _runMode,
                    _alertStream,
                    null,
                    _tradingStackLogger));
        }
    }
}
