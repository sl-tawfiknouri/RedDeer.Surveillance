namespace TestHarness.Tests.Engine.OrderGenerator
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    [TestFixture]
    public class TradingMarkovProcessTests
    {
        private ILogger _logger;

        private IStockExchangeStream _stockStream;

        private ITradeStrategy<Order> _tradeStrategy;

        private IOrderStream<Order> _tradeStream;

        private IDisposable _unsubscriber;

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradingMarketUpdateDrivenProcess(null, this._tradeStrategy));
        }

        [Test]
        public void Constructor_ConsidersNullStrategy_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradingMarketUpdateDrivenProcess(this._logger, null));
        }

        [Test]
        public void InitiateTrading_WithExistingTrading_DoesNotDeadLockOnTerminatingOldTrading()
        {
            var orderDataGenerator = new TradingMarketUpdateDrivenProcess(this._logger, this._tradeStrategy);

            orderDataGenerator.InitiateTrading(this._stockStream, this._tradeStream);
            orderDataGenerator.InitiateTrading(this._stockStream, this._tradeStream);

            A.CallTo(() => this._unsubscriber.Dispose()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._unsubscriber = A.Fake<IDisposable>();
            this._stockStream = A.Fake<IStockExchangeStream>();
            this._tradeStream = A.Fake<IOrderStream<Order>>();
            this._tradeStrategy = A.Fake<ITradeStrategy<Order>>();
            this._logger = A.Fake<ILogger>();

            A.CallTo(() => this._stockStream.Subscribe(A<IObserver<EquityIntraDayTimeBarCollection>>.Ignored))
                .Returns(this._unsubscriber);
        }
    }
}