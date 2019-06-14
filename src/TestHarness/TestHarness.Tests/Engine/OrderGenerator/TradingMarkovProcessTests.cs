﻿using FakeItEasy;
using NUnit.Framework;
using System;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Tests.Engine.OrderGenerator
{
    [TestFixture]
    public class TradingMarkovProcessTests
    {
        private IDisposable _unsubscriber;
        private IStockExchangeStream _stockStream;
        private IOrderStream<Order> _tradeStream;
        private ITradeStrategy<Order> _tradeStrategy;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _unsubscriber = A.Fake<IDisposable>();
            _stockStream = A.Fake<IStockExchangeStream>();
            _tradeStream = A.Fake<IOrderStream<Order>>();
            _tradeStrategy = A.Fake<ITradeStrategy<Order>>();
            _logger = A.Fake<ILogger>();

            A
                .CallTo(() => _stockStream.Subscribe(A<IObserver<EquityIntraDayTimeBarCollection>>.Ignored))
                .Returns(_unsubscriber);
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradingMarketUpdateDrivenProcess(null, _tradeStrategy));
        }

        [Test]
        public void Constructor_ConsidersNullStrategy_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradingMarketUpdateDrivenProcess(_logger, null));
        }

        [Test]
        public void InitiateTrading_WithExistingTrading_DoesNotDeadLockOnTerminatingOldTrading()
        {
            var orderDataGenerator = new TradingMarketUpdateDrivenProcess(_logger, _tradeStrategy);

            orderDataGenerator.InitiateTrading(_stockStream, _tradeStream);
            orderDataGenerator.InitiateTrading(_stockStream, _tradeStream);

            A
                .CallTo(() => _unsubscriber.Dispose())
                .MustHaveHappenedOnceExactly();
        }
    }
}