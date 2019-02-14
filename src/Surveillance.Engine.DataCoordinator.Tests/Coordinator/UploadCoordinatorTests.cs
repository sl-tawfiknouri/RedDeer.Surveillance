using System;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator;

namespace Surveillance.Engine.DataCoordinator.Tests.Coordinator
{
    [TestFixture]
    public class UploadCoordinatorTests
    {
        private IRuleConfiguration _ruleConfiguration;

        [SetUp]
        public void Setup()
        {
            _ruleConfiguration = A.Fake<IRuleConfiguration>();
        }

        [Test]
        public void Constructor_RuleConfiguration_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UploadCoordinator(null));
        }

    }
}
