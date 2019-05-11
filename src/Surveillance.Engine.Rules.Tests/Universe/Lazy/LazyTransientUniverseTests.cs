using System;
using Domain.Surveillance.Scheduling;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Lazy;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Universe.Lazy
{
    [TestFixture]
    public class LazyTransientUniverseTests
    {
        private ILazyScheduledExecutioner _lazyScheduledExecutioner;
        private ISystemProcessOperationContext _opCtx;
        private IUniverseBuilder _universeBuilder;

        [SetUp]
        public void Setup()
        {
            _lazyScheduledExecutioner = new LazyScheduledExecutioner();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _universeBuilder = A.Fake<IUniverseBuilder>();
        }

        [Test]
        public void Testy()
        {
            var execution = new ScheduledExecution();

            var lol = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);

            foreach (var ohYah in lol)
            {
                Console.WriteLine($"HOT DAMN MAN! {ohYah.StateChange} {ohYah.UnderlyingEvent} {ohYah.EventTime}");
            }
        }
    }
}
