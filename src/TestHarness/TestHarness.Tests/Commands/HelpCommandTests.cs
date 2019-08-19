namespace TestHarness.Tests.Commands
{
    using NUnit.Framework;

    using TestHarness.Commands;

    [TestFixture]
    public class HelpCommandTests
    {
        [Test]
        public void Handles_ReturnsFalseFor_NullString()
        {
            var command = new HelpCommand();

            var result = command.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalseFor_RandomString()
        {
            var command = new HelpCommand();

            var result = command.Handles("9a-eg");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrueFor_LowerCaseHelp()
        {
            var command = new HelpCommand();

            var result = command.Handles("help");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsTrueFor_UpperCaseHelp()
        {
            var command = new HelpCommand();

            var result = command.Handles("HELP");

            Assert.IsTrue(result);
        }
    }
}