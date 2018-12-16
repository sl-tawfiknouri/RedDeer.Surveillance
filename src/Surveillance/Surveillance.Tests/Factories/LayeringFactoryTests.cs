using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        private IUniverseMarketCacheFactory _factory;
        private ILogger<LayeringRuleFactory> _logger;

        [SetUp]
        public void Setup()
        {
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _logger = A.Fake<ILogger<LayeringRuleFactory>>();
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_factory, null));
        }

        [Test]
        public void Constructor_ConsidersNullFactory_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(null, _logger));
        }
    }
}
