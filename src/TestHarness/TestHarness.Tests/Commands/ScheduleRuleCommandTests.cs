namespace TestHarness.Tests.Commands
{
    using System.Threading;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using FakeItEasy;

    using Infrastructure.Network.Aws.Interfaces;

    using NUnit.Framework;

    using TestHarness.Commands;
    using TestHarness.Configuration.Interfaces;
    using TestHarness.Display.Interfaces;
    using TestHarness.Factory.Interfaces;

    [TestFixture]
    public class ScheduleRuleCommandTests
    {
        private IAppFactory _appFactory;

        private IAwsQueueClient _awsQueueClient;

        private INetworkConfiguration _configuration;

        private IConsole _console;

        private IScheduledExecutionMessageBusSerialiser _serialiser;

        [Test]
        public void Handles_CommandWithRunScheduleRule_Substring()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            var result = scheduleRule.Handles("blah run schedule rule blah");

            Assert.IsTrue(result);
        }

        [Test]
        public void Handles_ReturnsFalse_IncorrectCommand()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            var result = scheduleRule.Handles("Help");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_NullCommand()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            var result = scheduleRule.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Run_QueuesMessageToBus_ForValidArguments()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            scheduleRule.Run("run schedule rule 01/09/2018 12/09/2018");

            A.CallTo(() => this._serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();

            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidDates()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            scheduleRule.Run("run schedule rule 01/02/2018 03/02/2018 01/09/2018");

            A.CallTo(() => this._console.WriteToUserFeedbackLine(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => this._serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidDatesTerminationPrecedesInitiation()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            scheduleRule.Run("run schedule rule 02/02/2018 01/02/2018");

            A.CallTo(() => this._console.WriteToUserFeedbackLine(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => this._serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidInitiationDates()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            scheduleRule.Run("run schedule rule ehh 01/09/2018");

            A.CallTo(() => this._console.WriteToUserFeedbackLine(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => this._serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidTerminationDates()
        {
            var scheduleRule = new ScheduleRuleCommand(this._appFactory);

            scheduleRule.Run("run schedule rule 01/09/2018 ehh");

            A.CallTo(() => this._console.WriteToUserFeedbackLine(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => this._serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A.CallTo(
                () => this._awsQueueClient.SendToQueue(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }

        [SetUp]
        public void Setup()
        {
            this._console = A.Fake<IConsole>();
            this._awsQueueClient = A.Fake<IAwsQueueClient>();
            this._serialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            this._configuration = A.Fake<INetworkConfiguration>();

            this._appFactory = A.Fake<IAppFactory>();

            A.CallTo(() => this._appFactory.Console).Returns(this._console);
            A.CallTo(() => this._appFactory.AwsQueueClient).Returns(this._awsQueueClient);
            A.CallTo(() => this._appFactory.ScheduledExecutionSerialiser).Returns(this._serialiser);
            A.CallTo(() => this._appFactory.Configuration).Returns(this._configuration);
        }
    }
}