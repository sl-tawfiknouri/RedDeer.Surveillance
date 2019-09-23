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

    [TestFixture]
    public class ScheduleDisassemblerTests
    {
        private IRuleParameterApi _apiRepository;

        private IQueueDistributedRulePublisher _distributedRulePublisher;

        private ILogger<ScheduleDisassembler> _logger;

        private ISystemProcessOperationContext _systemProcessOperationContext;

        [Test]
        public async Task Initiate_RuleRunForEightDays_JustRunsRuleWithTimeWindowOf10Days()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForEightDays_RunsRuleWithTimeWindowOf1DaysOverTwoExecutions()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 2);
        }

        [Test]
        public async Task Initiate_RuleRunForFourteenDays_JustRunsRuleWithTimeWindowOf1DaysThreeTimes()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 3);
        }

        [Test]
        public async Task Initiate_RuleRunForLessThanOneDay_JustRunsRule()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForOneHourOnly_JustRunsRule()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForOneYear_JustRunsRuleWithTimeWindowOfEightDaysSchedules22Runs()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 22);
        }

        [Test]
        public async Task Initiate_RuleRunForOneYear_JustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 52);
        }

        [Test]
        public async Task Initiate_RuleRunForSevenDays_JustRunsRule()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForSixDays_JustRunsRule()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForTwoWeeksButNullParams_DoesNotDoAnything()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(new RuleParameterDto { HighProfits = null });

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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        [Ignore("Not supporting this atm")]
        public async Task Initiate_RuleRunTwoRulesForOneYear_JustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A.CallTo(() => this._apiRepository.Get()).Returns(
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

            await scheduler.Disassemble(this._systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => this._apiRepository.Get()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 74);
        }

        [SetUp]
        public void Setup()
        {
            this._apiRepository = A.Fake<IRuleParameterApi>();
            this._systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            this._distributedRulePublisher = A.Fake<IQueueDistributedRulePublisher>();
            this._logger = A.Fake<ILogger<ScheduleDisassembler>>();
        }

        private ScheduleDisassembler Build()
        {
            return new ScheduleDisassembler(this._apiRepository, this._distributedRulePublisher, this._logger);
        }
    }
}