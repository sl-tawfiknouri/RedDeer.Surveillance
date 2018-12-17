﻿using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        private IMarketTradingHoursManager _tradingHoursManager;
        private IUniverseMarketCacheFactory _factory;
        private ILogger<LayeringRuleFactory> _logger;

        [SetUp]
        public void Setup()
        {
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = A.Fake<ILogger<LayeringRuleFactory>>();
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_tradingHoursManager, _factory, null));
        }

        [Test]
        public void Constructor_ConsidersNullFactory_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_tradingHoursManager, null, _logger));
        }
    }
}
