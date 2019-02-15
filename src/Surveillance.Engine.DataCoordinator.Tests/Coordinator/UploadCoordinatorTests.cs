using System;
using DomainV2.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator;

namespace Surveillance.Engine.DataCoordinator.Tests.Coordinator
{
    [TestFixture]
    public class UploadCoordinatorTests
    {
        private IRuleConfiguration _ruleConfiguration;
        private ILogger<DataVerifier> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleConfiguration = A.Fake<IRuleConfiguration>();
            _logger = new NullLogger<DataVerifier>();

            A.CallTo(() => _ruleConfiguration.AutoScheduleRules).Returns(true);
        }

        [Test]
        public void Constructor_RuleConfiguration_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(_ruleConfiguration, null));
        }

        [Test]
        public void AnalyseFileId_Null_Message_Returns()
        {
            var coordinator = new DataVerifier(_ruleConfiguration, _logger);

            Assert.DoesNotThrow(() => coordinator.Scan());
        }




    }
}
