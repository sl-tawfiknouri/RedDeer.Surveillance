namespace TestHarness.Tests.Commands
{
    using System;
    using System.IO;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Commands;
    using TestHarness.Factory.Interfaces;

    [TestFixture]
    public class DemoMarketEquityFileCommandTests
    {
        private IAppFactory _appFactory;

        [Test]
        public void Constructor_ConsidersNullAppFactory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DemoMarketEquityFileCommand(null));
        }

        [Test]
        public void Handles_ReturnsFalse_ForDifferentCaseCorrectCommandButNonExistantFile()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("RUN dEmO equity market file MarketDataEquityETC.csv");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NullCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_RandomCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("a random command");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForCorrectCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("run demo equity market file MarketDataEquity.csv");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForCorrectStopCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("stop demo equity market file");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForDifferentCaseCorrectCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("RUN dEmO equity market file MarketDataEquity.csv");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrue_ForDifferentCaseStopCommand()
        {
            var command = new DemoMarketEquityFileCommand(this._appFactory);

            var result = command.Handles("StOp dEmO equity market file");

            Assert.IsTrue(result);
        }

        [SetUp]
        public void Setup()
        {
            this._appFactory = A.Fake<IAppFactory>();
        }

        [OneTimeSetUp]
        public void Setup_OneTime()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoMarketEquityFileCommand.FileDirectory);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }
    }
}