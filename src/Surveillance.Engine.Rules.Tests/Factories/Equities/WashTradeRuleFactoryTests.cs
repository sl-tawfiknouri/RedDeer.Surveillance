using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    [TestFixture]
    public class WashTradeRuleFactoryTests
    {
        private ICurrencyConverter _currencyConverter;
        private IWashTradePositionPairer _positionPairer;
        private IClusteringService _clustering;
        private IUniverseEquityOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<WashTradeRule> _logger;
        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        private IWashTradeRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private IUniverseAlertStream _alertStream;

        [SetUp]
        public void Setup()
        {
            _currencyConverter = A.Fake<ICurrencyConverter>();
            _positionPairer = A.Fake<IWashTradePositionPairer>();
            _clustering = A.Fake<IClusteringService>();
            _orderFilter = A.Fake<IUniverseEquityOrderFilter>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = new NullLogger<WashTradeRule>();
            _tradingHistoryLogger = new NullLogger<TradingHistoryStack>();

            _equitiesParameters = A.Fake<IWashTradeRuleEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _alertStream = A.Fake<IUniverseAlertStream>();
        }

        [Test]
        public void Constructor_CurrencyConverter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    null,
                    _positionPairer, 
                    _clustering,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_PositionPairer_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    _currencyConverter,
                    null,
                    _clustering,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Clustering_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    _currencyConverter,
                    _positionPairer,
                    null,
                    _orderFilter,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_OrderFilter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    null,
                    _factory,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Factory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    _orderFilter,
                    null,
                    _logger,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
                    _currencyConverter,
                    _positionPairer,
                    _clustering,
                    _orderFilter,
                    _factory,
                    null,
                    _tradingHistoryLogger));
        }

        [Test]
        public void Constructor_TradingHistoryLogger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new EquityRuleWashTradeFactory(
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
            var factory = new EquityRuleWashTradeFactory(
                _currencyConverter,
                _positionPairer,
                _clustering,
                _orderFilter,
                _factory,
                _logger,
                _tradingHistoryLogger);

            Assert.Throws<ArgumentNullException>(() => factory.Build(_equitiesParameters, null, _alertStream, RuleRunMode.ForceRun));
        }

        [Test]
        public void Build_Parameters_Null_Throws_Exception()
        {
            var factory = new EquityRuleWashTradeFactory(
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
            var factory = new EquityRuleWashTradeFactory(
                _currencyConverter,
                _positionPairer,
                _clustering,
                _orderFilter,
                _factory,
                _logger,
                _tradingHistoryLogger);

            var result = factory.Build(_equitiesParameters, _ruleCtx, _alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<WashTradeRule>(result);
        }
    }
}
