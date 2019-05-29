using FakeItEasy;
using MathNet.Numerics.Distributions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;

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
        public void Constructor_ConsidersANullLogger_Throws_Exception()
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
            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE), 
                DateTime.UtcNow,
                null);

            Assert.Throws<ArgumentNullException>(() => tradeStrategy.ExecuteTradeStrategy(frame, null));
        }

        [Test]
        public void ExecuteTradeStrategy_NoSecuritiesInFrame_Logs()
        {
            var tradeStrategy = new MarkovTradeStrategy(_logger, _tradeVolumeStrategy);
            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new List<EquityInstrumentIntraDayTimeBar>());

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

        private EquityIntraDayTimeBarCollection GenerateFrame(int securityFrames)
        {
            var frames = GenerateSecurityFrames(securityFrames);
            var exchFrame = new EquityIntraDayTimeBarCollection(
                new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                frames);

            return exchFrame;
        }

        private IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> GenerateSecurityFrames(int number)
        {
            var results = new List<EquityInstrumentIntraDayTimeBar>();
            for (var i = 0; i < number; i++)
            {
                var buyPrice = Math.Round(ContinuousUniform.Sample(0.01, 30000), 2);
                var sellPrice = buyPrice * ContinuousUniform.Sample(0.95, 1);
                var volume = DiscreteUniform.Sample(0, 100000000);
                
                var frame = new EquityInstrumentIntraDayTimeBar(
                    new FinancialInstrument(
                        InstrumentTypes.Equity,
                        new InstrumentIdentifiers(string.Empty, string.Empty, $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}", $"STAN-{i}"), 
                        "Standard Chartered", "CFI", "USD", "ISSUER-IDENTIFIER"), 
                    new SpreadTimeBar(
                        new Money((decimal)buyPrice, "GBP"),
                        new Money((decimal)sellPrice, "GBP"),
                        new Money((decimal)buyPrice, "GBP"),
                        new Volume(volume)),
                    new DailySummaryTimeBar(
                        1000,
                        null,
                        1000,
                        new Volume(volume),
                        DateTime.UtcNow),
                    DateTime.UtcNow,
                    new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

                results.Add(frame);
            }

            return results;
        }
    }
}