using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Factories;
using Surveillance.Rules.Layering.Interfaces;

namespace Surveillance.Tests.Factories
{
    [TestFixture]
    public class LayeringFactoryTests
    {
        private ILogger<LayeringRuleFactory> _logger;
        private ILayeringCachedMessageSender _layeringCachedMessageSender;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<LayeringRuleFactory>>();
            _layeringCachedMessageSender = A.Fake<ILayeringCachedMessageSender>();
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(_layeringCachedMessageSender, null));
        }

        [Test]
        public void Constructor_ConsidersNullMessageSender_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRuleFactory(null, _logger));
        }
    }
}
