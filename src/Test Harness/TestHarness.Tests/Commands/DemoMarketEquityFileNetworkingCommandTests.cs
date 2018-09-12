using System;
using FakeItEasy;
using NUnit.Framework;
using TestHarness.Commands;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class DemoMarketEquityFileNetworkingCommandTests
    {
        private IAppFactory _appFactory;

        [SetUp]
        public void Setup()
        {
            _appFactory = A.Fake<IAppFactory>();
        }

        [Test]
        public void Constructor_ConsidersNullAppFactory_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DemoMarketEquityFileNetworkingCommand(null));
        }

        [Test]
        public void Handles_ReturnsFalse_NullCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_RandomCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("a random command");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForCorrectCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("run demo equity market file networking MarketDataEquity.csv");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForDifferentCaseCorrectCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("RUN dEmO equity market file networking MarketDataEquity.csv");

            Assert.IsTrue(result);
        }
        
        [Test]
        public void Handles_ReturnsFalse_ForDifferentCaseCorrectCommandButNonExistantFile()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("RUN dEmO equity market file networking MarketDataEquityETC.csv");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForCorrectStopCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("stop demo equity market file networking");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForDifferentCaseStopCommand()
        {
            var command = new DemoMarketEquityFileNetworkingCommand(_appFactory);

            var result = command.Handles("StOp dEmO equity market file networking");

            Assert.IsTrue(result);
        }
    }
}
