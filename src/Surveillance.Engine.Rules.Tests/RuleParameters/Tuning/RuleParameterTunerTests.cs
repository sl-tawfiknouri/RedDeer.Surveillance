namespace Surveillance.Engine.Rules.Tests.RuleParameters.Tuning
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.RuleParameters.Equities;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.RuleParameters.Tuning;

    [TestFixture]
    public class RuleParameterTunerTests
    {
        private ILogger<RuleParameterTuner> _logger;

        [Test]
        public void Ctor_Null_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleParameterTuner(null));
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<RuleParameterTuner>>();
        }

        [Test]
        public void Tuner_AdjustHighProfitFixedIncomeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new HighProfitsRuleFixedIncomeParameters(
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
                false,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(22));
        }

        [Test]
        public void Tuner_AdjustHighVolumeIssuanceFixedIncomeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new HighVolumeIssuanceRuleFixedIncomeParameters(
                "id",
                TimeSpan.FromHours(1),
                10m,
                3m,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(5));
        }

        [Test]
        public void Tuner_AdjustPlacingOrderWithNoIntentField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new PlacingOrderWithNoIntentToExecuteRuleEquitiesParameters(
                "id",
                0.3m,
                TimeSpan.FromHours(1),
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(11));
        }

        [Test]
        public void Tuner_AdjustRampingRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new RampingRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                2,
                0.3m,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(22));
        }

        [Test]
        public void Tuner_AdjustsCancelledOrderField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new CancelledOrderRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                0.2m,
                2,
                4,
                new ClientOrganisationalFactors[0],
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(27));
        }

        [Test]
        public void Tuner_AdjustsHighProfitsField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new HighProfitsRuleEquitiesParameters(
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
                false,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(22));
        }

        [Test]
        public void Tuner_AdjustsHighVolumeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new HighVolumeRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                0.6m,
                0.4m,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(23));
        }

        [Test]
        public void Tuner_AdjustsLayeringRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new LayeringRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                0.4m,
                true,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(18));
        }

        [Test]
        public void Tuner_AdjustsMarkingTheCloseField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new MarkingTheCloseEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                0.4m,
                0.7m,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(23));
        }

        [Test]
        public void Tuner_AdjustSpoofingRuleField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new SpoofingRuleEquitiesParameters(
                "id",
                TimeSpan.FromHours(1),
                0.3m,
                0.8m,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(16));
        }

        [Test]
        public void Tuner_AdjustWashTradeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new WashTradeRuleEquitiesParameters(
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
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(31));
        }

        [Test]
        public void Tuner_AdjustWashTradeFixedIncomeField_AsExpected()
        {
            var tuner = new RuleParameterTuner(this._logger);

            var ruleParameters = new WashTradeRuleFixedIncomeParameters(
                "id",
                TimeSpan.FromHours(1),
                true,
                true,
                3,
                0.6m,
                0,
                "GBP",
                2,
                4,
                null,
                null,
                null,
                null,
                null,
                null,
                true,
                true);

            var result = tuner.ParametersFramework(ruleParameters);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(26));
        }
    }
}