using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Market;
using FakeItEasy;
using MathNet.Numerics.Distributions;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator.Strategies;

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
                new List<SecurityFrame>());

            tradeStrategy.ExecuteTradeStrategy(frame, _tradeOrderStream);

            _logger.Log(
                LogLevel.Info,
                "No securities were present on the exchange frame in the probabilistic trade strategy");
        }


        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void ExecuteTradeStategy_RecordsTrades_100Iterations(int frames)
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
                frames);

            return exchFrame;
        }

        private IReadOnlyCollection<SecurityFrame> GenerateSecurityFrames(int number)
        {
            var results = new List<SecurityFrame>();
            for (var i = 0; i < number; i++)
            {
                var buyPrice = Math.Round(ContinuousUniform.Sample(0.01, 30000), 2);
                var sellPrice = buyPrice * ContinuousUniform.Sample(0.95, 1);
                var volume = DiscreteUniform.Sample(0, 100000000);
                
                var frame = new SecurityFrame(
                    new Domain.Equity.Security(
                        new Domain.Equity.Security.SecurityId($"STAN-{i}"), "Standard Chartered", "STAN"),
                    new Spread(new Price((decimal)buyPrice), new Price((decimal)sellPrice)),
                    new Volume(volume));

                results.Add(frame);
            }

            return results;
        }
    }
}