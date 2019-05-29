using NUnit.Framework;
using TestHarness.Commands;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class UnRecognisedCommandTests
    {
        [Test]
        public void Handles_Null_Command()
        {
            var unRecognisedCommand = new UnRecognisedCommand();

            var result = unRecognisedCommand.Handles(null);

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_Empty_Command()
        {
            var unRecognisedCommand = new UnRecognisedCommand();

            var result = unRecognisedCommand.Handles(string.Empty);

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_DoesNotHandle_NonEmptyCommand()
        {
            var unRecognisedCommand = new UnRecognisedCommand();

            var result = unRecognisedCommand.Handles("a");

            Assert.IsFalse(result);
        }
    }
}
