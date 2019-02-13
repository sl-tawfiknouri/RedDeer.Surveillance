using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.WashTrade;
using Surveillance.Engine.Rules.Rules.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories
{
    [TestFixture]
    public class WashTradeRuleFactoryTests
    {
        private ICurrencyConverter _currencyConverter;
        private IWashTradePositionPairer _positionPairer;
        private IWashTradeClustering _clustering;
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<WashTradeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IWashTradeRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _currencyConverter = A.Fake<ICurrencyConverter>();
            _positionPairer = A.Fake<IWashTradePositionPairer>();
            _clustering = A.Fake<IWashTradeClustering>();
            _orderFilter = A.Fake<IUniverseOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<WashTradeRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _parameters = A.Fake<IWashTradeRuleParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_CurrencyConverter_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    null,
                    _positionPairer, 
                    _clustering,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_PositionPairer_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    null,
                    _clustering,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Clustering_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    null,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_OrderFilter_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    null,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Factory_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    _orderFilter,
                    null,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    _orderFilter,
                    _factory,
                    null,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_TradingHistoryLogger_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new WashTradeRuleFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    _orderFilter,
                    _factory,
                    _logger,
                    null));
        }

        [Test]
        public void Build_RuleCtx_Null_Throws_Exception()
        {
            var factory = new WashTradeRuleFactory(
                _currencyConverter,
                _positionPairer,
                _clustering,
                _orderFilter,
                _factory,
                _logger,
                _tradingHistoryLogger);

            Assert.Throws<ArgumentNullException>(() => factory.Build(_parameters, null, _alertStream, RuleRunMode.ForceRun));
        }

        [Test]
        public void Build_Parameters_Null_Throws_Exception()
        {
            var factory = new WashTradeRuleFactory(
                _currencyConverter,
                _positionPairer,
                _clustering,
                _orderFilter,
                _factory,
                _logger,
                _tradingHistoryLogger);

            Assert.Throws<ArgumentNullException>(() => factory.Build(null, _ruleCtx, _alertStream, RuleRunMode.ForceRun));
        }

        [Test]
        public void Build_Returns_A_WashTrade_Rule()
        {
            var factory = new WashTradeRuleFactory(
                _currencyConverter,
                _positionPairer,
                _clustering,
                _orderFilter,
                _factory,
                _logger,
                _tradingHistoryLogger);

            var result = factory.Build(_parameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<WashTradeRule>(result);
        }
    }
}
