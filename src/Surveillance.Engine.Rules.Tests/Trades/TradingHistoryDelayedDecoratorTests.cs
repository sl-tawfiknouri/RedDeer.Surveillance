namespace Surveillance.Engine.Rules.Tests.Trades
{
    using System;

    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;

    [TestFixture]
    public class TradingHistoryDelayedDecoratorTests
    {
        private ITradingHistoryStack _tradingHistoryStack;

        [Test]
        public void DelayedDecorator_DoesAddToDecoratee_IfTimeSpanOneDayAndTimeMovesForwardTwoDays()
        {
            var delayedDecorator = new TradingHistoryDelayedDecorator(this._tradingHistoryStack, TimeSpan.FromDays(1));
            var order = A.Fake<Order>();

            delayedDecorator.Add(order, DateTime.Now);

            A.CallTo(() => this._tradingHistoryStack.Add(A<Order>.Ignored, A<DateTime>.Ignored)).MustNotHaveHappened();

            delayedDecorator.ArchiveExpiredActiveItems(DateTime.Now.AddDays(2));

            A.CallTo(() => this._tradingHistoryStack.Add(A<Order>.Ignored, A<DateTime>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void DelayedDecorator_DoesAddToDecoratee_IfTimeSpanZero()
        {
            var delayedDecorator = new TradingHistoryDelayedDecorator(this._tradingHistoryStack, TimeSpan.Zero);
            var order = A.Fake<Order>();

            delayedDecorator.Add(order, DateTime.Now);

            A.CallTo(() => this._tradingHistoryStack.Add(A<Order>.Ignored, A<DateTime>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void DelayedDecorator_DoesNotAddToDecoratee_IfTimeSpanOneDay()
        {
            var delayedDecorator = new TradingHistoryDelayedDecorator(this._tradingHistoryStack, TimeSpan.FromDays(1));
            var order = A.Fake<Order>();

            delayedDecorator.Add(order, DateTime.Now);

            A.CallTo(() => this._tradingHistoryStack.Add(A<Order>.Ignored, A<DateTime>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void DelayedDecorator_ThrowsForNull_Decoratee()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradingHistoryDelayedDecorator(null, TimeSpan.Zero));
        }

        [SetUp]
        public void Setup()
        {
            this._tradingHistoryStack = A.Fake<ITradingHistoryStack>();
        }
    }
}