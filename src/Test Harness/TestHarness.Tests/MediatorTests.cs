using FakeItEasy;
using NLog;
using NUnit.Framework;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IEquityDataGenerator _equityDataGenerator;
        private IAppFactory _appFactory;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _equityDataGenerator = A.Fake<IEquityDataGenerator>();
            _appFactory = A.Fake<IAppFactory>();
            _logger = A.Fake<ILogger>();

            A.CallTo(() => _appFactory.Logger).Returns(_logger);
        }

        [Test]
        public void Constructor_DoesNotConsiderNullFactory_ToBeExceptional()
        {
            Assert.DoesNotThrow(() => new Mediator(null));
        }

        [Test]
        public void Terminate_LogsTermination_AndCallsTerminateWalk()
        {
            var mediator = new Mediator(_appFactory);

            mediator.Initiate();
            mediator.Terminate();

            A
                .CallTo(() => _appFactory.Logger.Log(LogLevel.Info, "Mediator Terminating"))
                .MustHaveHappenedOnceExactly();
        }
    }
}
