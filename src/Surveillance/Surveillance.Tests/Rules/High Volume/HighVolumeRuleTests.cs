using System;
using Domain.Scheduling;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Rules.High_Volume
{
    [TestFixture]
    public class HighVolumeRuleTests
    {
        private IHighVolumeRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _opCtx;
        private ILogger<IHighVolumeRule> _logger;

        [SetUp]
        public void Setup()
        {
            _parameters = A.Fake<IHighVolumeRuleParameters>();
            _opCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _logger = A.Fake<ILogger<IHighVolumeRule>>();
        }

        [Test]
        public void Constructor_ConsidersNullParameters_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(null, _opCtx, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullOpCtx_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_parameters, null, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_parameters, _opCtx, null));
        }

        [Test]
        public void Eschaton_UpdatesAlertCountAndEndsEvent_ForCtx()
        {
            var highVolumeRule = new HighVolumeRule(_parameters, _opCtx, _logger);

            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _opCtx.UpdateAlertEvent(A<int>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _opCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Eschaton()
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, underlyingEvent);
        }
    }
}
