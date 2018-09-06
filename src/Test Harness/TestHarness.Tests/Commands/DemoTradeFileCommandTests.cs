using System.IO;
using FakeItEasy;
using NUnit.Framework;
using TestHarness.Commands;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class DemoTradeFileCommandTests
    {
        private IAppFactory _appFactory;

        [SetUp]
        public void Setup_Iteration()
        {
            _appFactory = A.Fake<IAppFactory>();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        [Test]
        public void Handles_ReturnsFalse_ForNullCommand()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(_appFactory);

            var result = demoTradeFileCommand.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_CorrectCommandWrongCsvFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(_appFactory);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);
            var file = Path.Combine(directory, "myFile2.csv");
            File.Create(file);

            var result = demoTradeFileCommand.Handles("run demo trade file networking myFile3.csv");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrue_CorrectCommandCsvFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(_appFactory);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);
            var file = Path.Combine(directory, "anyFile.csv");
            File.Create(file);

            var result = demoTradeFileCommand.Handles("run demo trade file networking anyFile.csv");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NoFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(_appFactory);

            var result = demoTradeFileCommand.Handles("run demo trade file networking  ");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NotCorrectFileType()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(_appFactory);

            var result = demoTradeFileCommand.Handles("run demo trade file networking crazy.jpeg");

            Assert.IsFalse(result);
        }
    }
}
