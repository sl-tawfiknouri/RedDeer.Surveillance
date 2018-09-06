using System;
using NUnit.Framework;
using TestHarness.Commands;
using TestHarness.State;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class QuitCommandTests
    {
        [Test]
        public void Constructor_NullProgramState_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new QuitCommand(null));
        }

        [Test]
        public void Run_SetsProgramStateExecution_ToFalse()
        {
            var state = new ProgramState {Executing = true};
            var command = new QuitCommand(state);

            command.Run(string.Empty);

            Assert.IsFalse(state.Executing);
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
        public void Handles_HandlesquitCommand_UpperCase()
        {
            var state = new ProgramState { Executing = true };
            var command = new QuitCommand(state);

            var result = command.Handles("QUIT");

            Assert.IsTrue(result);
        }
    }
}
