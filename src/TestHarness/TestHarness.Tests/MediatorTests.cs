namespace TestHarness.Tests
{
    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestHarness.Factory.Interfaces;

    [TestFixture]
    public class MediatorTests
    {
        private IAppFactory _appFactory;

        private ILogger _logger;

        [Test]
        public void Constructor_DoesNotConsiderNullFactory_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Mediator(null));
        }

        [SetUp]
        public void Setup()
        {
            this._appFactory = A.Fake<IAppFactory>();
            this._logger = A.Fake<ILogger>();

            A.CallTo(() => this._appFactory.Logger).Returns(this._logger);
        }
    }
}