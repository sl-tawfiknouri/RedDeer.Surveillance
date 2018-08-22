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
        public void Initiate_NullCommand_Logs()
        {
            var mediator = new Mediator(_appFactory);

            mediator.Initiate();

            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Mediator Initiating"))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => _logger.Log(LogLevel.Warn, "Mediator receieved a null initiation command"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_CallsAppFactoryBuild()
        {
            var mediator = new Mediator(_appFactory);

            mediator.Initiate();

            A
                .CallTo(() => _appFactory.Build())
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_WalkAlreadyInitiated_TerminatesOldWalk()
        {
            var mediator = new Mediator(_appFactory);

            mediator.Initiate();
            mediator.Initiate();

            A
                .CallTo(() => _appFactory.Build())
                .MustHaveHappenedTwiceExactly();

            A
                .CallTo(() => _equityDataGenerator.TerminateWalk())
                .MustHaveHappenedOnceExactly();
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

            A
                .CallTo(() => _equityDataGenerator.TerminateWalk())
                .MustHaveHappenedOnceExactly();

        }
    }
}
