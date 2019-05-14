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
    public class LazyTransientUniverseFactoryTests
    {
        private IUniverseBuilder _universeBuilder;
        private ILazyScheduledExecutioner _scheduledExecutioner;
        private ISystemProcessOperationContext _opCtx;

        [SetUp]
        public void Setup()
        {
            _universeBuilder = A.Fake<IUniverseBuilder>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _scheduledExecutioner = A.Fake<ILazyScheduledExecutioner>();
        }

        [Test]
        public void Ctor_NullUniverseBuilder_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LazyTransientUniverseFactory(null, _scheduledExecutioner));
        }

        [Test]
        public void Ctor_NullScheduledExecutioner_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LazyTransientUniverseFactory(_universeBuilder, null));
        }

        [Test]
        public void Build_Returns_Universe()
        {
            var factory = new LazyTransientUniverseFactory(_universeBuilder, _scheduledExecutioner);
            var execution = new ScheduledExecution();

            var universe = factory.Build(execution, _opCtx);

            Assert.IsNotNull(universe);
        }
    }
}
