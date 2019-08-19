namespace Surveillance.Engine.Rules.Tests.Factories.Equities
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class CancelledOrderRuleFactoryTests
    {
        private IUniverseAlertStream _alertStream;

        private IUniverseMarketCacheFactory _factory;

        private ILogger<CancelledOrderRule> _logger;

        private IUniverseEquityOrderFilterService _orderFilterService;

        private ICancelledOrderRuleEquitiesParameters _parameters;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private ILogger<TradingHistoryStack> _tradingHistoryLogger;

        [Test]
        public void Build_Returns_A_Cancelled_Order_Rule()
        {
            var factory = new EquityRuleCancelledOrderFactory(
                this._orderFilterService,
                this._factory,
                this._logger,
                this._tradingHistoryLogger);

            var result = factory.Build(this._parameters, this._ruleCtx, this._alertStream, RuleRunMode.ForceRun);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Constructor_Factory_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleCancelledOrderFactory(
                    this._orderFilterService,
                    null,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleCancelledOrderFactory(
                    this._orderFilterService,
                    this._factory,
                    null,
                    this._tradingHistoryLogger));
        }

        [Test]
        public void Constructor_Order_Filter_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new EquityRuleCancelledOrderFactory(
                    null,
                    this._factory,
                    this._logger,
                    this._tradingHistoryLogger));
        }

        [SetUp]
        public void Setup()
        {
            this._orderFilterService = A.Fake<IUniverseEquityOrderFilterService>();
            this._factory = A.Fake<IUniverseMarketCacheFactory>();
            this._logger = A.Fake<ILogger<CancelledOrderRule>>();
            this._tradingHistoryLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._parameters = A.Fake<ICancelledOrderRuleEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._alertStream = A.Fake<IUniverseAlertStream>();
        }
    }
}