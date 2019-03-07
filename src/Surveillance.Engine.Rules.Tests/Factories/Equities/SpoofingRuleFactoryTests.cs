using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class SpoofingRuleFactoryTests
    {
        private IUniverseEquityOrderFilterService _orderFilterService;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<SpoofingRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private ISpoofingRuleEquitiesParameters _spoofingEquitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<SpoofingRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _spoofingEquitiesParameters = A.Fake<ISpoofingRuleEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_Null_Factory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleSpoofingFactory(null, _orderFilterService, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_OrderFilter_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleSpoofingFactory(_factory, null, _logger, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleSpoofingFactory(_factory, _orderFilterService, null, _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Null_TradingHistoryLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityRuleSpoofingFactory(_factory, _orderFilterService, _logger, null));
        }

        [Test]
        public void Build_Returns_Non_Null_Rule()
        {
            var factory = new EquityRuleSpoofingFactory(_factory, _orderFilterService, _logger, _tradingHistoryLogger);

            var result = factory.Build(_spoofingEquitiesParameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }
    }
}
