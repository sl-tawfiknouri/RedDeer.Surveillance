using FakeItEasy;
using MathNet.Numerics.Distributions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Tests.Engine.OrderGenerator.Strategies
{
    [TestFixture]
    public class ProbabilisticTradeStrategyTests
    {
        private ILogger _logger;
        private IOrderStream<Order> _tradeOrderStream;
        private ITradeVolumeStrategy _tradeVolumeStrategy;

        [SetUp]
        public void Setup()
        {
            _tradeOrderStream = A.Fake<IOrderStream<Order>>();
            _logger = A.Fake<ILogger>();

            _tradeVolumeStrategy = new TradeVolumeNormalDistributionStrategy(6);
        }

        [Test]
        public void Constructor_ConsidersANullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
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
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE), 
                DateTime.UtcNow,
                null);

            Assert.Throws<ArgumentNullException>(() => tradeStrategy.ExecuteTradeStrategy(frame, null));
        }

        [Test]
        public void ExecuteTradeStrategy_NoSecuritiesInFrame_Logs()
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);
            var frame = new ExchangeFrame(
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new List<SecurityTick>());

            tradeStrategy.ExecuteTradeStrategy(frame, _tradeOrderStream);

            _logger.LogInformation("No securities were present on the exchange frame in the probabilistic trade strategy");
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
                .CallTo(() => _tradeOrderStream.Add(A<Order>.Ignored))
                .Invokes(x => Console.WriteLine(x.Arguments[0]));

            tradeStrategy.ExecuteTradeStrategy(frame, _tradeOrderStream);

            Assert.IsTrue(frames >= 0);
        }

        private ExchangeFrame GenerateFrame(int securityFrames)
        {
            var frames = GenerateSecurityFrames(securityFrames);
            var exchFrame = new ExchangeFrame(
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
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
                    new FinancialInstrument(
                        InstrumentTypes.Equity,
                        new InstrumentIdentifiers(string.Empty, $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}"), 
                        "Standard Chartered", "CFI", "ISSUER-IDENTIFIER"), 
                    new Spread(new CurrencyAmount((decimal)buyPrice, "GBP"), new CurrencyAmount((decimal)sellPrice, "GBP"), new CurrencyAmount((decimal)buyPrice, "GBP")),
                    new Volume(volume),
                    new Volume(volume),
                    DateTime.UtcNow,
                    3000,
                    null,
                    1000,
                    new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

                results.Add(frame);
            }

            return results;
        }
    }
}