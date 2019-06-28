using System;
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
        public void Tuner_AdjustsField_AsExpected()
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
        }
    }
}
