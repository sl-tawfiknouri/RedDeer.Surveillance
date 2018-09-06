using NUnit.Framework;
using TestHarness.Commands;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class UnrecognisedCommandTests
    {
        [Test]
        public void Handles_Null_Command()
        {
            var unrecognisedCommand = new UnrecognisedCommand();

            var result = unrecognisedCommand.Handles(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_Empty_Command()
        {
            var unrecognisedCommand = new UnrecognisedCommand();

            var result = unrecognisedCommand.Handles(string.Empty);

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_DoesNotHandle_NonEmptyCommand()
        {
            var unrecognisedCommand = new UnrecognisedCommand();

            var result = unrecognisedCommand.Handles("a");

            Assert.IsFalse(result);
        }
    }
}
