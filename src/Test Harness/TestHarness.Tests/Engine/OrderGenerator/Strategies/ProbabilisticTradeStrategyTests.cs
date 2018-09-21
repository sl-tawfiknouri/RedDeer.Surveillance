using Domain.Market;
using FakeItEasy;
using MathNet.Numerics.Distributions;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Tests.Engine.OrderGenerator.Strategies
{
    [TestFixture]
    public class ProbabilisticTradeStrategyTests
    {
        private ILogger _logger;
        private ITradeOrderStream<TradeOrderFrame> _tradeOrderStream;
        private ITradeVolumeStrategy _tradeVolumeStrategy;

        [SetUp]
        public void Setup()
        {
            _tradeOrderStream = A.Fake<ITradeOrderStream<TradeOrderFrame>>();
            _logger = A.Fake<ILogger>();

            _tradeVolumeStrategy = new TradeVolumeNormalDistributionStrategy(6);
        }

        [Test]
        public void Constructor_ConsidersANullLogger_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new MarkovTradeStrategy(null, _tradeVolumeStrategy));
        }

        [Test]
        public void ExecuteTradeStrategy_NullTick_DoesNotThrow()
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);

            Assert.DoesNotThrow(() => tradeStrategy.ExecuteTradeStrategy(null, _tradeOrderStream));
        }

        [Test]
        public void ExecuteTradeStrategy_NullTradeOrders_DoesThrow()
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);
            var frame = new ExchangeFrame(
                new StockExchange(
                    new Market.MarketId("LSE"), "London Stock Exchange"), 
                DateTime.UtcNow,
                null);

            Assert.Throws<ArgumentNullException>(() => tradeStrategy.ExecuteTradeStrategy(frame, null));
        }

        [Test]
        public void ExecuteTradeStrategy_NoSecuritiesInFrame_Logs()
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);
            var frame = new ExchangeFrame(
                new StockExchange(
                    new Market.MarketId("LSE"), "London Stock Exchange"),
                DateTime.UtcNow,
                new List<SecurityTick>());

            tradeStrategy.ExecuteTradeStrategy(frame, _tradeOrderStream);

            _logger.Log(
                LogLevel.Info,
                "No securities were present on the exchange frame in the probabilistic trade strategy");
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ExecuteTradeStrategy_RecordsTrades_100Iterations(int frames)
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);
            var frame = GenerateFrame(frames);

            A
                .CallTo(() => _tradeOrderStream.Add(A<TradeOrderFrame>.Ignored))
                .Invokes(x => Console.WriteLine(x.Arguments[0]));

            A
                .CallTo(() => _logger.Log(A<LogLevel>.Ignored, A<string>.Ignored))
                .Invokes(x => Console.WriteLine(x.Arguments[1]));

            tradeStrategy.ExecuteTradeStrategy(frame, _tradeOrderStream);

            Assert.IsTrue(frames >= 0);
        }

        private ExchangeFrame GenerateFrame(int securityFrames)
        {
            var frames = GenerateSecurityFrames(securityFrames);
            var exchFrame = new ExchangeFrame(
                new StockExchange(new Market.MarketId("LSE"), "London Stock Exchange"),
                DateTime.UtcNow,
                frames);

            return exchFrame;
        }

        private IReadOnlyCollection<SecurityTick> GenerateSecurityFrames(int number)
        {
            var results = new List<SecurityTick>();
            for (var i = 0; i < number; i++)
            {
                var buyPrice = Math.Round(ContinuousUniform.Sample(0.01, 30000), 2);
                var sellPrice = buyPrice * ContinuousUniform.Sample(0.95, 1);
                var volume = DiscreteUniform.Sample(0, 100000000);
                
                var frame = new SecurityTick(
                    new Security(
                        new SecurityIdentifiers($"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}"),
                        "Standard Chartered", "CFI"),
                    new Spread(new Price((decimal)buyPrice, "GBP"), new Price((decimal)sellPrice, "GBP"), new Price((decimal)buyPrice, "GBP")),
                    new Volume(volume),
                    DateTime.UtcNow,
                    3000,
                    null,
                    1000);

                results.Add(frame);
            }

            return results;
        }
    }
}