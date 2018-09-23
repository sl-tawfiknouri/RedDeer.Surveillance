using System.Threading;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using FakeItEasy;
using NUnit.Framework;
using TestHarness.Commands;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display;
using TestHarness.Display.Interfaces;
using TestHarness.Factory.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace TestHarness.Tests.Commands
{
    [TestFixture]
    public class ScheduleRuleCommandTests
    {
        private IConsole _console;
        private IAwsQueueClient _awsQueueClient;
        private IScheduledExecutionMessageBusSerialiser _serialiser;
        private INetworkConfiguration _configuration;
        private IAppFactory _appFactory;

        [SetUp]
        public void Setup()
        {
            _console = A.Fake<IConsole>();
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _serialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _configuration = A.Fake<INetworkConfiguration>();

            _appFactory = A.Fake<IAppFactory>();

            A.CallTo(() => _appFactory.Console).Returns(_console);
            A.CallTo(() => _appFactory.AwsQueueClient).Returns(_awsQueueClient);
            A.CallTo(() => _appFactory.ScheduledExecutionSerialiser).Returns(_serialiser);
            A.CallTo(() => _appFactory.Configuration).Returns(_configuration);
        }

        [Test]
        public void Handles_ReturnsFalse_NullCommand()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            var result = scheduleRule.Handles(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_ReturnsFalse_IncorrectCommand()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            var result = scheduleRule.Handles("Help");

            Assert.IsFalse(result);
        }

        [Test]
        public void Handles_CommandWithRunScheduleRule_Substring()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            var result = scheduleRule.Handles("blah run schedule rule blah");

            Assert.IsTrue(result);
        }

        [Test]
        public void Run_QueuesMessageToBus_ForValidArguments()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            scheduleRule.Run("run schedule rule 01/09/2018 12/09/2018");

            A
                .CallTo(() =>
                    _serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() => 
                    _awsQueueClient.SendToQueue(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidTerminationDates()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            scheduleRule.Run("run schedule rule 01/09/2018 ehh");

            A
                .CallTo(() =>
                    _console.WriteToUserFeedbackLine(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() =>
                    _serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A
                .CallTo(() =>
                    _awsQueueClient.SendToQueue(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidInitiationDates()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            scheduleRule.Run("run schedule rule ehh 01/09/2018");

            A
                .CallTo(() =>
                    _console.WriteToUserFeedbackLine(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() =>
                    _serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A
                .CallTo(() =>
                    _awsQueueClient.SendToQueue(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidDates()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            scheduleRule.Run("run schedule rule 01/02/2018 03/02/2018 01/09/2018");

            A
                .CallTo(() =>
                    _console.WriteToUserFeedbackLine(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() =>
                    _serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A
                .CallTo(() =>
                    _awsQueueClient.SendToQueue(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void Run_QueuesMessageToBus_ReturnsScreenErrorForInvalidDatesTerminationPrecedesInitiation()
        {
            var scheduleRule =
                new ScheduleRuleCommand(_appFactory);

            scheduleRule.Run("run schedule rule 02/02/2018 01/02/2018");

            A
                .CallTo(() =>
                    _console.WriteToUserFeedbackLine(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            A
                .CallTo(() =>
                    _serialiser.SerialiseScheduledExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();

            A
                .CallTo(() =>
                    _awsQueueClient.SendToQueue(
                        A<string>.Ignored,
                        A<string>.Ignored,
                        A<CancellationToken>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
