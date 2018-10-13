using System;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.Layering;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Layering
{
    [TestFixture]
    public class LayeringRuleTests
    {
        private ILogger _logger;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ILayeringRuleParameters _parameters;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _parameters = A.Fake<ILayeringRuleParameters>();
        }

        [Test]
        public void Constructor_NullParametersConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(null, _logger, _ruleCtx));
        }

        [Test]
        public void Constructor_NullLoggerConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, null, _ruleCtx));
        }

        [Test]
        public void Constructor_NullRuleContextConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _logger, null));
        }

        [Test]
        public void EndOfUniverse_RecordUpdateAlertAndEndEvent()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(A<int>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// This test isn't relevant to the final analysis but will get us there
        /// </summary>
        [Test]
        public void RunRule_RaisesAlertInEschaton_WhenBidirectionalTrade()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(1)).MustHaveHappenedOnceExactly();
        }
    }
}
