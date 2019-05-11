using System;
using System.Threading.Tasks;
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
        public void Enumerate_WhenEmptyUniverse_ReturnsEmptyEnumeration()
        {
            var execution = new ScheduledExecution();

            A
                .CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored, A<ISystemProcessOperationContext>.Ignored,
                A<bool>.Ignored, A<bool>.Ignored))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(null, null, null, null)));

            var lazyCollection = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);
            
            foreach (var _ in lazyCollection)
            {
                Assert.Fail("Should of been empty collection and not able to access enumeration");
            }

            Assert.True(true);
        }
    }
}
