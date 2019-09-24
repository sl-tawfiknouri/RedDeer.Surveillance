namespace Surveillance.Engine.Rules.Tests.Universe.Lazy
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy;
    using Surveillance.Data.Universe.Lazy.Interfaces;

    [TestFixture]
    public class LazyTransientUniverseTests
    {
        private ILazyScheduledExecutioner _lazyScheduledExecutioner;

        private ISystemProcessOperationContext _opCtx;

        private IUniverseBuilder _universeBuilder;

        [Test]
        public void Enumerate_WhenEmptyUniverse_ReturnsEmptyEnumeration()
        {
            var execution = new ScheduledExecution();

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(null)));

            var lazyCollection = new LazyTransientUniverse(
                this._lazyScheduledExecutioner,
                this._universeBuilder,
                execution,
                this._opCtx);

            foreach (var _ in lazyCollection)
                Assert.Fail("Should of been empty collection and not able to access enumeration");

            Assert.True(true);
        }

        [Test]
        public void Enumerate_WhenFiveEvents_OverTwoFetches_EnumeratesFiveOnly()
        {
            var initialDate = new DateTimeOffset(2019, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var terminalDate = new DateTimeOffset(2019, 01, 09, 0, 0, 0, TimeSpan.Zero);
            var execution = new ScheduledExecution
                                {
                                    TimeSeriesInitiation = initialDate, TimeSeriesTermination = terminalDate
                                };

            var universeEventColl1 = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            var universeEventColl2 = new[]
                                         {
                                             A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>(),
                                             A.Fake<IUniverseEvent>()
                                         };

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl1)));

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(
                this._lazyScheduledExecutioner,
                this._universeBuilder,
                execution,
                this._opCtx);

            var tracker = 5;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        [Test]
        public void Enumerate_WhenOnlyGenesisAndEschaton_EnumeratesTwiceOnly()
        {
            var execution = new ScheduledExecution();

            var universeEventColl = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl)));

            var lazyCollection = new LazyTransientUniverse(
                this._lazyScheduledExecutioner,
                this._universeBuilder,
                execution,
                this._opCtx);

            var tracker = 2;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        [Test]
        public void Enumerate_WhenSevenEvents_OverThreeFetches_EnumeratesSevenOnly()
        {
            var initialDate = new DateTimeOffset(2019, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var terminalDate = new DateTimeOffset(2019, 01, 19, 0, 0, 0, TimeSpan.Zero);
            var execution = new ScheduledExecution
                                {
                                    TimeSeriesInitiation = initialDate, TimeSeriesTermination = terminalDate
                                };

            var universeEventColl1 = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            var universeEventColl2 = new[]
                                         {
                                             A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>(),
                                             A.Fake<IUniverseEvent>()
                                         };

            var universeEventColl3 = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl1)));

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl3)));

            A.CallTo(
                () => this._universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(
                this._lazyScheduledExecutioner,
                this._universeBuilder,
                execution,
                this._opCtx);

            var tracker = 7;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        [SetUp]
        public void Setup()
        {
            this._lazyScheduledExecutioner = new LazyScheduledExecutioner();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._universeBuilder = A.Fake<IUniverseBuilder>();
        }
    }
}