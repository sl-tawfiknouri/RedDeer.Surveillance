namespace Surveillance.Engine.RuleDistributor.Tests.Distributor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.RuleDistributor.Distributor;
    using Surveillance.Engine.RuleDistributor.Queues.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;

    /// <summary>
    /// The schedule disassembler tests.
    /// </summary>
    [TestFixture]
    public class ScheduleDisassemblerTests
    {
        /// <summary>
        /// The application programming interface repository.
        /// </summary>
        private IRuleParameterApi apiRepository;

        /// <summary>
        /// The distributed rule publisher.
        /// </summary>
        private IQueueDistributedRulePublisher distributedRulePublisher;

        /// <summary>
        /// The system process operation context.
        /// </summary>
        private ISystemProcessOperationContext systemProcessOperationContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<ScheduleDisassembler> logger;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.apiRepository = A.Fake<IRuleParameterApi>();
            this.systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            this.distributedRulePublisher = A.Fake<IQueueDistributedRulePublisher>();
            this.logger = A.Fake<ILogger<ScheduleDisassembler>>();
        }

        /// <summary>
        /// The initiate rule run for eight days just runs rule with time window of 10 days.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForEightDaysJustRunsRuleWithTimeWindowOf10Days()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                          {
                              new HighVolumeRuleParameterDto
                                  {
                                      Id = "abc", WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                                  }
                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 10)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate rule run for eight days runs rule with time window of 1 days over two executions.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForEightDaysRunsRuleWithTimeWindowOf1DaysOverTwoExecutions()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                      {
                          new HighVolumeRuleParameterDto
                              {
                                  Id = "abc", WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                              }
                      }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 8)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 2);
        }

        /// <summary>
        /// The initiate rule run for fourteen days just runs rule with time window of 1 days three times.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForFourteenDaysJustRunsRuleWithTimeWindowOf1DaysThreeTimes()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                                          {
                                              new HighVolumeRuleParameterDto
                                                  {
                                                      Id = "abc", WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                                                  }
                                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 14)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 3);
        }

        /// <summary>
        /// The initiate rule run for less than one day just runs rule.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForLessThanOneDayJustRunsRule()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                          {
                              new HighVolumeRuleParameterDto
                                  {
                                      Id = "abc", WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                                  }
                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 01)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate rule run for one hour only just runs rule.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForOneHourOnlyJustRunsRule()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                          {
                              new HighVolumeRuleParameterDto
                                  {
                                      Id = "abc", WindowSize = new TimeSpan(0, 5, 0, 0, 0)
                                  }
                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01, 9, 0, 0),
                                    TimeSeriesTermination = new DateTime(2018, 01, 01, 9, 0, 0)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate rule run for one year just runs rule with time window of eight days schedules 22 runs.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForOneYearJustRunsRuleWithTimeWindowOfEightDaysSchedules22Runs()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                                          {
                                              new HighVolumeRuleParameterDto
                                                  {
                                                      Id = "abc", WindowSize = new TimeSpan(8, 0, 0, 0, 0)
                                                  }
                                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 22);
        }

        /// <summary>
        /// The initiate rule run for one year just runs rule with time window of three days schedules 22 runs.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForOneYearJustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                                          {
                                              new HighVolumeRuleParameterDto
                                                  {
                                                      Id = "abc", WindowSize = new TimeSpan(3, 0, 0, 0, 0)
                                                  }
                                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 52);
        }

        /// <summary>
        /// The initiate rule run for seven days just runs rule.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForSevenDaysJustRunsRule()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                                          {
                                              new HighVolumeRuleParameterDto
                                                  {
                                                      Id = "abc", WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                                                  }
                                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 07)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate rule run for six days just runs rule.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForSixDaysJustRunsRule()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighVolumes = new[]
                                          {
                                              new HighVolumeRuleParameterDto
                                                  {
                                                      Id = "abc", WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                                                  }
                                          }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 06)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The initiate rule run for two weeks but null params does not do anything.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task InitiateRuleRunForTwoWeeksButNullParamsDoesNotDoAnything()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(new RuleParameterDto { HighProfits = null });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighProfits, Ids = new string[0] }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2018, 01, 15)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();
        }

        /// <summary>
        /// The initiate rule run two rules for one year just runs rule with time window of three days schedules 22 runs.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        [Ignore("Not supporting this atm")]
        public async Task InitiateRuleRunTwoRulesForOneYearJustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A.CallTo(() => this.apiRepository.Get()).Returns(
                new RuleParameterDto
                    {
                        HighProfits =
                            new[] { new HighProfitsRuleParameterDto { WindowSize = new TimeSpan(3, 0, 0, 0, 0) } },
                        CancelledOrders = new[]
                                              {
                                                  new CancelledOrderRuleParameterDto
                                                      {
                                                          WindowSize = new TimeSpan(08, 0, 0, 0, 0)
                                                      }
                                              }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));

            var scheduler = this.Build();

            var execution = new ScheduledExecution
                                {
                                    Rules = new List<RuleIdentifier>
                                                {
                                                    new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] },
                                                    new RuleIdentifier
                                                        {
                                                            Rule = Rules.CancelledOrders, Ids = new string[0]
                                                        }
                                                },
                                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(this.systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this.apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 74);
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="ScheduleDisassembler"/>.
        /// </returns>
        private ScheduleDisassembler Build()
        {
            return new ScheduleDisassembler(this.apiRepository, this.distributedRulePublisher, this.logger);
        }
    }
}