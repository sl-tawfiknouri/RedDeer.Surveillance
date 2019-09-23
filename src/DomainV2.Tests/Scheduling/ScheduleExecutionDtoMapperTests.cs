namespace Domain.Tests.Scheduling
{
    using System;
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using RuleIdentifier = RedDeer.Contracts.SurveillanceService.Rules.RuleIdentifier;
    using Rules = RedDeer.Contracts.SurveillanceService.Rules.Rules;
    using ScheduledExecution = RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution;

    [TestFixture]
    public class ScheduleExecutionDtoMapperTests
    {
        private ILogger<ScheduleExecutionDtoMapper> _logger;

        [Test]
        public void MapToDomain_Maps_AsExpected()
        {
            var mapper = new ScheduleExecutionDtoMapper(this._logger);

            var initialDto = new ScheduledExecution
                                 {
                                     CorrelationId = "Correlation-12345",
                                     IsBackTest = true,
                                     TimeSeriesInitiation = new DateTimeOffset(),
                                     TimeSeriesTermination = new DateTimeOffset(),
                                     Rules = new List<RuleIdentifier>
                                                 {
                                                     new RuleIdentifier
                                                         {
                                                             Ids = new[] { "back test-1" }, Rule = Rules.HighProfits
                                                         }
                                                 }
                                 };

            var result = mapper.MapToDomain(initialDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.CorrelationId, initialDto.CorrelationId);
            Assert.AreEqual(result.IsBackTest, initialDto.IsBackTest);
            Assert.AreEqual(result.TimeSeriesInitiation, initialDto.TimeSeriesInitiation);
            Assert.AreEqual(result.TimeSeriesTermination, initialDto.TimeSeriesTermination);
            Assert.AreEqual(result.Rules.Count, initialDto.Rules.Count);
            Assert.AreEqual(result.Rules[0].Ids, new[] { "back test-1" });
            Assert.AreEqual(result.Rules[0].Rule, Surveillance.Scheduling.Rules.HighProfits);
        }

        [Test]
        public void MapToDto_Maps_AsExpected()
        {
            var mapper = new ScheduleExecutionDtoMapper(this._logger);

            var initialDto = new Surveillance.Scheduling.ScheduledExecution
                                 {
                                     CorrelationId = "Correlation-12345",
                                     IsBackTest = true,
                                     TimeSeriesInitiation = new DateTimeOffset(),
                                     TimeSeriesTermination = new DateTimeOffset(),
                                     Rules = new List<Surveillance.Scheduling.RuleIdentifier>
                                                 {
                                                     new Surveillance.Scheduling.RuleIdentifier
                                                         {
                                                             Ids = new[] { "back test-2" },
                                                             Rule = Surveillance.Scheduling.Rules.Layering
                                                         }
                                                 }
                                 };

            var result = mapper.MapToDto(initialDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.CorrelationId, initialDto.CorrelationId);
            Assert.AreEqual(result.IsBackTest, initialDto.IsBackTest);
            Assert.AreEqual(result.TimeSeriesInitiation, initialDto.TimeSeriesInitiation);
            Assert.AreEqual(result.TimeSeriesTermination, initialDto.TimeSeriesTermination);
            Assert.AreEqual(result.Rules.Count, initialDto.Rules.Count);
            Assert.AreEqual(result.Rules[0].Ids, new[] { "back test-2" });
            Assert.AreEqual(result.Rules[0].Rule, Rules.Layering);
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<ScheduleExecutionDtoMapper>>();
        }
    }
}