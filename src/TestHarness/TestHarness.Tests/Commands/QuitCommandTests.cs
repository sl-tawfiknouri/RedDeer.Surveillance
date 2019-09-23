namespace TestHarness.Tests.Commands
{
    using System;

    using NUnit.Framework;

    using TestHarness.Commands;
    using TestHarness.State;

    [TestFixture]
    public class QuitCommandTests
    {
        [Test]
        public void Constructor_NullProgramState_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new QuitCommand(null));
        }

        [Test]
        public void Handles_DoesNotHandle_Null()
        {
            var state = new ProgramState { Executing = true };
            var command = new QuitCommand(state);

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_HandlesQuitCommand_LowerCase()
        {
            var state = new ProgramState { Executing = true };
            var command = new QuitCommand(state);

            var result = command.Handles("quit");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_HandlesQuitCommand_UpperCase()
        {
            var state = new ProgramState { Executing = true };
            var command = new QuitCommand(state);

            var result = command.Handles("QUIT");

            Assert.IsTrue(result);
        }

        [Test]
        public void Run_SetsProgramStateExecution_ToFalse()
        {
            var state = new ProgramState { Executing = true };
            var command = new QuitCommand(state);

            command.Run(string.Empty);

            Assert.IsFalse(state.Executing);
        }
    }
}