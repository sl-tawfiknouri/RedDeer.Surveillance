﻿using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.Rules.RuleParameters.Equities;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.RuleParameters.Tuning;

namespace Surveillance.Engine.Rules.Tests.RuleParameters.Tuning
{
    [TestFixture]
    public class RuleParameterTunerTests
    {
        private ILogger<RuleParameterTuner> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<RuleParameterTuner>>();
        }

        [Test]
        public void Ctor_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleParameterTuner(null));
        }

        [Test]
        public void Tuner_AdjustsCancelledOrderField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new CancelledOrderRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    0.2m,
                    2,
                    4,
                    new ClientOrganisationalFactors[0],
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(39));
        }

        [Test]
        public void Tuner_AdjustsHighProfitsField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new HighProfitsRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1), 
                    TimeSpan.FromHours(1),
                    true,
                    true,
                    0.3m,
                    0.2m,
                    false,
                    "gbp",
                    null,
                    false);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(32));
        }

        [Test]
        public void Tuner_AdjustsHighVolumeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new HighVolumeRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    0.6m,
                    0.4m,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(33));
        }

        [Test]
        public void Tuner_AdjustsLayeringRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new LayeringRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    0.4m,
                    true,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(28));
        }

        [Test]
        public void Tuner_AdjustsMarkingTheCloseField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new MarkingTheCloseEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    0.4m,
                    0.7m,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(33));
        }

        [Test]
        public void Tuner_AdjustPlacingOrderWithNoIntentField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
                    "id",
                    0.3m,
                    TimeSpan.FromHours(1),
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(21));
        }

        [Test]
        public void Tuner_AdjustRampingRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new RampingRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    2,
                    0.3m,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(33));
        }

        [Test]
        public void Tuner_AdjustSpoofingRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new SpoofingRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    0.3m,
                    0.8m,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(27));
        }

        [Test]
        public void Tuner_AdjustWashTradeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(_logger);

            var ruleParameters =
                new WashTradeRuleEquitiesParameters(
                    "id",
                    TimeSpan.FromHours(1),
                    true,
                    true,
                    2,
                    8,
                    0.3m,
                    "GBP",
                    4,
                    0.2m,
                    null,
                    true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(45));
        }
    }
}
