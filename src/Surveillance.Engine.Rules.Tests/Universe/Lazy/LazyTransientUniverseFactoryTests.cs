namespace Surveillance.Engine.Rules.Tests.Universe.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy;
    using Surveillance.Data.Universe.Lazy.Interfaces;

    [TestFixture]
    public class LazyTransientUniverseFactoryTests
    {
        private ISystemProcessOperationContext _opCtx;

        private ILazyScheduledExecutioner _scheduledExecutioner;

        private IUniverseBuilder _universeBuilder;

        [Test]
        public void Build_Returns_Universe()
        {
            var factory = new LazyTransientUniverseFactory(this._universeBuilder, this._scheduledExecutioner);
            var execution = new ScheduledExecution();

            var universe = factory.Build(execution, this._opCtx);

            Assert.IsNotNull(universe);
        }

        [Test]
        public void Ctor_NullScheduledExecutioner_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LazyTransientUniverseFactory(this._universeBuilder, null));
        }

        [Test]
        public void Ctor_NullUniverseBuilder_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new LazyTransientUniverseFactory(null, this._scheduledExecutioner));
        }

        [SetUp]
        public void Setup()
        {
            this._universeBuilder = A.Fake<IUniverseBuilder>();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._scheduledExecutioner = A.Fake<ILazyScheduledExecutioner>();
        }
    }
}