namespace Surveillance.Data.Universe.Tests.Lazy
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;

    /// <summary>
    /// The lazy transient universe tests.
    /// </summary>
    [TestFixture]
    public class LazyTransientUniverseTests
    {
        /// <summary>
        /// The data manifest interpreter.
        /// </summary>
        private IDataManifestInterpreter dataManifestInterpreter;

        /// <summary>
        /// The operation context.
        /// </summary>
        private ISystemProcessOperationContext operationContext;

        /// <summary>
        /// The universe builder.
        /// </summary>
        private IUniverseBuilder universeBuilder;

        /// <summary>
        /// The enumerate when empty universe returns empty enumeration.
        /// </summary>
        [Test]
        public void EnumerateWhenEmptyUniverseReturnsEmptyEnumeration()
        {
            var execution = new ScheduledExecution();

            A.CallTo(
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(null)));

            var lazyCollection = new LazyTransientUniverse(
                this.universeBuilder,
                execution,
                this.operationContext,
                this.dataManifestInterpreter);

            foreach (var _ in lazyCollection)
                Assert.Fail("Should of been empty collection and not able to access enumeration");

            Assert.True(true);
        }

        /// <summary>
        /// The enumerate when five events over two fetches enumerates five only.
        /// </summary>
        [Test]
        public void EnumerateWhenFiveEventsOverTwoFetchesEnumeratesFiveOnly()
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
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl1)));

            A.CallTo(
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(
                this.universeBuilder,
                execution,
                this.operationContext,
                this.dataManifestInterpreter);

            var tracker = 5;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        /// <summary>
        /// The enumerate when only genesis and eschaton enumerates twice only.
        /// </summary>
        [Test]
        public void EnumerateWhenOnlyGenesisAndEschatonEnumeratesTwiceOnly()
        {
            var execution = new ScheduledExecution();

            var universeEventColl = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            A.CallTo(
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl)));

            var lazyCollection = new LazyTransientUniverse(
                this.universeBuilder,
                execution,
                this.operationContext,
                this.dataManifestInterpreter);

            var tracker = 2;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        /// <summary>
        /// The enumerate when seven events over three fetches enumerates seven only.
        /// </summary>
        [Test]
        public void EnumerateWhenSevenEventsOverThreeFetchesEnumeratesSevenOnly()
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
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl1)));

            A.CallTo(
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    false,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl3)));

            A.CallTo(
                () => this.universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true,
                    A<DateTimeOffset?>.Ignored,
                    A<DateTimeOffset?>.Ignored)).Returns(Task.FromResult((IUniverse)new Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(
                this.universeBuilder,
                execution,
                this.operationContext,
                this.dataManifestInterpreter);

            var tracker = 7;
            foreach (var _ in lazyCollection) tracker--;

            if (tracker != 0) Assert.Fail("Enumerated over an unexpected number of items");
            else Assert.True(true);
        }

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.dataManifestInterpreter = A.Fake<IDataManifestInterpreter>();
            this.operationContext = A.Fake<ISystemProcessOperationContext>();
            this.universeBuilder = A.Fake<IUniverseBuilder>();
        }
    }
}