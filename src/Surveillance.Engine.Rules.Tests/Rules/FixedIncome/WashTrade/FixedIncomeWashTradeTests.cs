using System;
using Domain.Core.Trading.Factories;
using Domain.Core.Trading.Factories.Interfaces;
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
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Rules.FixedIncome.WashTrade
{
    [TestFixture]
    public class FixedIncomeWashTradeTests
    {
        private RuleRunMode _runMode = RuleRunMode.ForceRun;
        private IWashTradeRuleFixedIncomeParameters _parameters;
        private IUniverseFixedIncomeOrderFilter _fixedIncomeOrderFile;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseMarketCacheFactory _marketCacheFactory;
        private IUniverseAlertStream _alertStream;
        private IClusteringService _clusteringService;
        private IPortfolioFactory _portfolioFactory;
        private ILogger<FixedIncomeHighProfitsRule> _logger;
        private ILogger<TradingHistoryStack> _tradingStackLogger;

        [SetUp]
        public void Setup()
        {
            _parameters = A.Fake<IWashTradeRuleFixedIncomeParameters>();
            _fixedIncomeOrderFile = A.Fake<IUniverseFixedIncomeOrderFilter>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _marketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _clusteringService = A.Fake<IClusteringService>();
            _portfolioFactory = A.Fake<IPortfolioFactory>();
            _logger = new NullLogger<FixedIncomeHighProfitsRule>();
            _tradingStackLogger = new NullLogger<TradingHistoryStack>();
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new FixedIncomeWashTradeRule(
                    _parameters,
                    _fixedIncomeOrderFile,
                    _ruleCtx,
                    _marketCacheFactory,
                    _runMode,
                    _alertStream,
                    _clusteringService,
                    _portfolioFactory,
                    null,
                    _tradingStackLogger));
        }
    }
}
