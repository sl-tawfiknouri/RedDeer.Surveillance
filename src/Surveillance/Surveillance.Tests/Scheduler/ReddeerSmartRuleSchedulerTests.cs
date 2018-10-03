﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Scheduler;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerSmartRuleSchedulerTests
    {
        private IAwsQueueClient _awsClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _busSerialiser;
        private IRuleParameterApiRepository _apiRepository;
        private ILogger<ReddeerDistributedRuleScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _awsClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _busSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _apiRepository = A.Fake<IRuleParameterApiRepository>();
            _logger = A.Fake<ILogger<ReddeerDistributedRuleScheduler>>();
        }

        [Test]
        public void Constructor_ConsidersNullAwsClient_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerDistributedRuleScheduler(
                    null,
                    _awsConfiguration,
                    _busSerialiser,
                    _apiRepository,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullAwsConfiguration_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    null,
                    _busSerialiser,
                    _apiRepository,
                    _logger));
        }


        [Test]
        public void Constructor_ConsidersNullMessageBusSerialiser_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    null,
                    _apiRepository,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullApiRepository_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    null,
                    _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    _busSerialiser,
                    _apiRepository,
                    null));
        }

        [Test]
        public async Task Initiate_RuleRunForSixDays_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler = 
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> {Domain.Scheduling.Rules.HighProfits},
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 06)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);
            
            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustNotHaveHappened();
            A
                .CallTo(() => _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForTwoWeeksButNullParams_JustRunsRuleForTwoWeeks()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = null
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 15)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappened();
            A
                .CallTo(() => _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForLessThanOneDay_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustNotHaveHappened();
            A
                .CallTo(() => _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForSevenDays_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 07)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustNotHaveHappened();
            A
                .CallTo(() => _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForEightDays_JustRunsRuleWithTimeWindowOf10Days()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 10)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForEightDays_RunsRuleWithTimeWindowOf1DaysOverTwoExecutions()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 8)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() =>
                    _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 2);
        }

        [Test]
        public async Task Initiate_RuleRunForFourteenDays_JustRunsRuleWithTimeWindowOf1DaysThreeTimes()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 14)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() =>
                    _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 3);
        }

        [Test]
        public async Task Initiate_RuleRunForOneYear_JustRunsRuleWithTimeWindowOfEightDaysSchedules22Runs()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(8, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() =>
                    _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 22);
        }

        [Test]
        public async Task Initiate_RuleRunForOneYear_JustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(3, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() =>
                    _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 52);
        }

        [Test]
        public async Task Initiate_RuleRunTwoRulesForOneYear_JustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(3, 0, 0, 0, 0)
                        },
                        CancelledOrder = new CancelledOrderRuleParameterDto
                        {
                            WindowSize = new TimeSpan(08, 0, 0, 0, 0)
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser();

            var scheduler =
                new ReddeerDistributedRuleScheduler(
                    _awsClient,
                    _awsConfiguration,
                    serialiser,
                    _apiRepository,
                    _logger);

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<Domain.Scheduling.Rules> { Domain.Scheduling.Rules.HighProfits, Domain.Scheduling.Rules.CancelledOrders },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.ExecuteNonDistributedMessage("any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() =>
                    _awsClient.SendToQueue(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 74);
        }
    }
}
