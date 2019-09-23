namespace TestHarness.Tests.Commands
{
    using System;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Commands;
    using TestHarness.Commands.Interfaces;
    using TestHarness.State.Interfaces;

    [TestFixture]
    public class InitiateCommandTests
    {
        private ICommandManager _commandManager;

        private IProgramState _programState;

        [Test]
        public void Constructor_NullCommandManager_ConsideredThrows_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new InitiateCommand(this._programState, null));
        }

        [Test]
        public void Constructor_NullProgramState_ConsideredThrows_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new InitiateCommand(null, this._commandManager));
        }

        [Test]
        public void Handles_EmptyCommandReturns_False()
        {
            var command = new InitiateCommand(this._programState, this._commandManager);

            var result = command.Handles(string.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_InitiateLowerCaseCommandReturns_True()
        {
            var command = new InitiateCommand(this._programState, this._commandManager);

            var result = command.Handles("initiate");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_InitiateUpperCaseCommandReturns_True()
        {
            var command = new InitiateCommand(this._programState, this._commandManager);

            var result = command.Handles("INITIATE");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_NullCommandReturns_False()
        {
            var command = new InitiateCommand(this._programState, this._commandManager);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_RandomCommandReturns_False()
        {
            var command = new InitiateCommand(this._programState, this._commandManager);

            // ReSharper disable once StringLiteralTypo
            var result = command.Handles("ajrah");

            Assert.IsFalse(result);
        }

        [SetUp]
        public void Setup()
        {
            this._programState = A.Fake<IProgramState>();
            this._commandManager = A.Fake<ICommandManager>();
        }
    }
}