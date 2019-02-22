﻿using System;
using System.Collections.Generic;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RuleIdentifier = RedDeer.Contracts.SurveillanceService.Rules.RuleIdentifier;
using ScheduledExecution = RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution;

namespace Domain.Tests.Scheduling
{
    [TestFixture]
    public class ScheduleExecutionDtoMapperTests
    {
        private ILogger<ScheduleExecutionDtoMapper> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ScheduleExecutionDtoMapper>>();
        }

        [Test]
        public void MapToDomain_Maps_AsExpected()
        {
            var mapper = new ScheduleExecutionDtoMapper(_logger);

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
                        Ids = new [] {"back test-1"},
                        Rule = RedDeer.Contracts.SurveillanceService.Rules.Rules.HighProfits
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
            Assert.AreEqual(result.Rules[0].Ids, new [] { "back test-1" });
            Assert.AreEqual(result.Rules[0].Rule, Rules.HighProfits);
        }

        [Test]
        public void MapToDto_Maps_AsExpected()
        {
            var mapper = new ScheduleExecutionDtoMapper(_logger);

            var initialDto = new Domain.Scheduling.ScheduledExecution()
            {
                CorrelationId = "Correlation-12345",
                IsBackTest = true,
                TimeSeriesInitiation = new DateTimeOffset(),
                TimeSeriesTermination = new DateTimeOffset(),
                Rules = new List<Domain.Scheduling.RuleIdentifier>
                {
                    new Domain.Scheduling.RuleIdentifier
                    {
                        Ids = new [] {"back test-2"},
                        Rule = Rules.Layering
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
            Assert.AreEqual(result.Rules[0].Rule, RedDeer.Contracts.SurveillanceService.Rules.Rules.Layering);          
        }
    }
}