﻿namespace TestHarness.Tests.Commands
{
    using System.IO;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Commands;
    using TestHarness.Factory.Interfaces;

    [TestFixture]
    public class DemoTradeFileCommandTests
    {
        private IAppFactory _appFactory;

        [Test]
        public void Handles_ReturnsFalse_CorrectCommandWrongCsvFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(this._appFactory);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);
            var file = Path.Combine(directory, "myFile2.csv");
            File.Create(file);

            var result = demoTradeFileCommand.Handles("run demo trade file myFile3.csv");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_ForNullCommand()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(this._appFactory);

            var result = demoTradeFileCommand.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NoFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(this._appFactory);

            var result = demoTradeFileCommand.Handles("run demo trade file  ");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NotCorrectFileType()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(this._appFactory);

            var result = demoTradeFileCommand.Handles("run demo trade file crazy.jpeg");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsTrue_CorrectCommandCsvFile()
        {
            var demoTradeFileCommand = new DemoTradeFileCommand(this._appFactory);

            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);
            var file = Path.Combine(directory, "anyFile.csv");
            File.Create(file);

            var result = demoTradeFileCommand.Handles("run demo trade file anyFile.csv");

            Assert.IsTrue(result);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), DemoTradeFileCommand.FileDirectory);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        [SetUp]
        public void Setup_Iteration()
        {
            this._appFactory = A.Fake<IAppFactory>();
        }
    }
}