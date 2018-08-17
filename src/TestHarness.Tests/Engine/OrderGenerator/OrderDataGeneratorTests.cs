using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using FakeItEasy;
using NLog;
using NUnit.Framework;
using System;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Tests.Engine.OrderGenerator
{
    [TestFixture]
    public class OrderDataGeneratorTests
    {
        private IDisposable _unsubscriber;
        private IStockExchangeStream _stockStream;
        private ITradeOrderStream _tradeStream;
        private ITradeStrategy _tradeStrategy;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _unsubscriber = A.Fake<IDisposable>();
            _stockStream = A.Fake<IStockExchangeStream>();
            _tradeStream = A.Fake<ITradeOrderStream>();
            _tradeStrategy = A.Fake<ITradeStrategy>();
            _logger = A.Fake<ILogger>();

            A
                .CallTo(() => _stockStream.Subscribe(A<IObserver<ExchangeFrame>>.Ignored))
                .Returns(_unsubscriber);
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderDataGenerator(null, _tradeStrategy));
        }

        [Test]
        public void Constructor_ConsidersNullStrategy_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderDataGenerator(_logger, null));
        }

        [Test]
        public void InitiateTrading_WithNullStockStream_LogsError()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.InitiateTrading(null, _tradeStream);

            A
                .CallTo(() => _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null stock stream"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InitiateTrading_WithNullTradeStream_LogsError()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.InitiateTrading(_stockStream, null);

            A
                .CallTo(() => _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null trade stream"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InitiateTrading_LogsInitiation()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.InitiateTrading(_stockStream, _tradeStream);

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator initiated with new stock stream"))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => _stockStream.Subscribe(A<IObserver<ExchangeFrame>>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void InitiateTrading_WithExistingTrading_DoesNotDeadLockOnTerminatingOldTrading()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.InitiateTrading(_stockStream, _tradeStream);
            orderDataGenerator.InitiateTrading(_stockStream, _tradeStream);

            A
                .CallTo(() => _unsubscriber.Dispose())
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => _logger.Log(LogLevel.Warn, "Initiating new trading with predecessor active"))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator initiated with new stock stream"))
                .MustHaveHappenedTwiceExactly();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator terminating trading"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void TerminateTrading_LogsOnTermination()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.TerminateTrading();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator terminating trading"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnCompleted_LogsMessageAndCallsTerminate()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.OnCompleted();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator received completed message from stock stream. Terminating order data generation"))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Order data generator terminating trading"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_LogsWhenException_IsNull()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);

            orderDataGenerator.OnError(null);

            A
                .CallTo(() => _logger.Log(LogLevel.Error, "Order data generator receieved a null exception OnError"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_LogsWhenException_IsPassedIn()
        {
            var orderDataGenerator = new OrderDataGenerator(_logger, _tradeStrategy);
            var exception = new StackOverflowException();

            orderDataGenerator.OnError(exception);

            A
                .CallTo(() => _logger.Log(LogLevel.Error, A<Exception>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}
