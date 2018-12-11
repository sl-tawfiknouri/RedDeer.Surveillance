using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IAppFactory _appFactory;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _appFactory = A.Fake<IAppFactory>();
            _logger = A.Fake<ILogger>();

            A.CallTo(() => _appFactory.Logger).Returns(_logger);
        }

        [Test]
        public void Constructor_DoesNotConsiderNullFactory_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Mediator(null));
        }
    }
}
