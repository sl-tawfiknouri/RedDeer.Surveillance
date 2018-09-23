using System;
using FakeItEasy;
using NUnit.Framework;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.State.Interfaces;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class InitiateCommandTests
    {
        private IProgramState _programState;
        private ICommandManager _commandManager;

        [SetUp]
        public void Setup()
        {
            _programState = A.Fake<IProgramState>();
            _commandManager = A.Fake<ICommandManager>();
        }

        [Test]
        public void Constructor_NullProgramState_ConsideredExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new InitiateCommand(null, _commandManager));
        }

        [Test]
        public void Constructor_NullCommandManager_ConsideredExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new InitiateCommand(_programState, null));
        }

        [Test]
        public void Handles_NullCommandReturns_False()
        {
            var command = new InitiateCommand(_programState, _commandManager);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_EmptyCommandReturns_False()
        {
            var command = new InitiateCommand(_programState, _commandManager);

            var result = command.Handles(string.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_RandomCommandReturns_False()
        {
            var command = new InitiateCommand(_programState, _commandManager);

            // ReSharper disable once StringLiteralTypo
            var result = command.Handles("ajrah");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_InitiateLowerCaseCommandReturns_True()
        {
            var command = new InitiateCommand(_programState, _commandManager);

            var result = command.Handles("initiate");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_InitiateUpperCaseCommandReturns_True()
        {
            var command = new InitiateCommand(_programState, _commandManager);

            var result = command.Handles("INITIATE");

            Assert.IsTrue(result);
        }
    }
}
