namespace TestHarness.Tests.Commands.Market_Abuse_Commands
{
    using System;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Commands.Market_Abuse_Commands;
    using TestHarness.Factory.Interfaces;

    [TestFixture]
    public class CancellationCommandTests
    {
        private IAppFactory _appFactory;

        [Test]
        public void Constructor_ThrowsExceptionOnNull_AppFactory()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new CancellationCommand(null));
        }

        [Test]
        public void Handles_ReturnsFalseFor_NullCommand()
        {
            var command = new CancellationCommand(this._appFactory);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrueFor_RunCancelledTrade()
        {
            var command = new CancellationCommand(this._appFactory);

            var result = command.Handles("run cancelled trade");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrueFor_RunCancelledTradeOddCasing()
        {
            var command = new CancellationCommand(this._appFactory);

            // ReSharper disable once StringLiteralTypo
            var result = command.Handles("RuN CanCelleD tRaDe");

            Assert.IsTrue(result);
        }

        [SetUp]
        public void Setup()
        {
            this._appFactory = A.Fake<IAppFactory>();
        }
    }
}