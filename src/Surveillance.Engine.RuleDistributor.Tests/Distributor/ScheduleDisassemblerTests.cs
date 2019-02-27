using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Engine.RuleDistributor.Distributor;
using Surveillance.Engine.RuleDistributor.Queues.Interfaces;

namespace Surveillance.Engine.RuleDistributor.Tests.Distributor
{
    [TestFixture]
    public class ScheduleDisassemblerTests
    {
        private IRuleParameterApiRepository _apiRepository;
        private ISystemProcessOperationContext _systemProcessOperationContext;
        private IQueueDistributedRulePublisher _distributedRulePublisher;
        private ILogger<ScheduleDisassembler> _logger;

        [SetUp]
        public void Setup()
        {
            _apiRepository = A.Fake<IRuleParameterApiRepository>();
            _systemProcessOperationContext = A.Fake<ISystemProcessOperationContext>();
            _distributedRulePublisher = A.Fake<IQueueDistributedRulePublisher>();
            _logger = A.Fake<ILogger<ScheduleDisassembler>>();
        }

        [Test]
        public async Task Initiate_RuleRunForSixDays_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 06)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForTwoWeeksButNullParams_DoesNotDoAnything()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = null
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighProfits, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 15)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Initiate_RuleRunForLessThanOneDay_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Initiate_RuleRunForOneHourOnly_JustRunsRule()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(0, 5, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01, 9, 0, 0),
                    TimeSeriesTermination = new DateTime(2018, 01, 01, 9, 0, 0)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 07)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                        {
                            Id = "abc",
                            WindowSize = new TimeSpan(10, 0, 0, 0, 0)
                        }
                    }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 10)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 8)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(1, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2018, 01, 14)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(8, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
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
                        HighVolumes = new[] { new HighVolumeRuleParameterDto()
                            {
                                Id = "abc",
                                WindowSize = new TimeSpan(3, 0, 0, 0, 0)
                            }
                        }
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            var scheduler = Build();
            
            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 52);
        }

        [Test]
        [Ignore("Not supporting this atm")]
        public async Task Initiate_RuleRunTwoRulesForOneYear_JustRunsRuleWithTimeWindowOfThreeDaysSchedules22Runs()
        {
            A
                .CallTo(() => _apiRepository.Get())
                .Returns(
                    new RuleParameterDto
                    {
                        HighProfits = new[] { new HighProfitsRuleParameterDto
                        {
                            WindowSize = new TimeSpan(3, 0, 0, 0, 0)
                        }},
                        CancelledOrders = new[] { new CancelledOrderRuleParameterDto
                        {
                            WindowSize = new TimeSpan(08, 0, 0, 0, 0)
                        }}
                    });

            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));

            var scheduler = Build();

            var execution =
                new ScheduledExecution
                {
                    Rules = new List<RuleIdentifier> { new RuleIdentifier { Rule = Rules.HighVolume, Ids = new string[0] }, new RuleIdentifier { Rule = Rules.CancelledOrders, Ids = new string[0] } },
                    TimeSeriesInitiation = new DateTime(2018, 01, 01),
                    TimeSeriesTermination = new DateTime(2019, 01, 01)
                };

            var messageBody = serialiser.SerialiseScheduledExecution(execution);

            await scheduler.Disassemble(_systemProcessOperationContext, execution, "any-id", messageBody);

            A.CallTo(() => _apiRepository.Get()).MustHaveHappenedOnceExactly();
            A
                .CallTo(() => _distributedRulePublisher.ScheduleExecution(A<ScheduledExecution>.Ignored))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 74);
        }

        private ScheduleDisassembler Build()
        {
            return new ScheduleDisassembler(
                _apiRepository,
                _distributedRulePublisher,
                _logger);

        }
    }
}
