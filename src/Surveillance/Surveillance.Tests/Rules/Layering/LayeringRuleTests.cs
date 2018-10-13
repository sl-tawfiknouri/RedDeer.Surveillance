using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.Layering;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Layering
{
    [TestFixture]
    public class LayeringRuleTests
    {
        private ILogger _logger;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ILayeringRuleParameters _parameters;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _parameters = A.Fake<ILayeringRuleParameters>();
        }

        [Test]
        public void Constructor_NullParametersConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(null, _logger, _ruleCtx));
        }

        [Test]
        public void Constructor_NullLoggerConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, null, _ruleCtx));
        }

        [Test]
        public void Constructor_NullRuleContextConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _logger, null));
        }

        [Test]
        public void EndOfUniverse_RecordUpdateAlertAndEndEvent()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(A<int>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }
    }
}
