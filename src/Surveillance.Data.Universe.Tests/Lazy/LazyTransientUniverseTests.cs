namespace Surveillance.Data.Universe.Tests.Lazy
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using FakeItEasy;

    using NUnit.Framework;

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
        /// The enumerate when empty universe returns empty enumeration.
        /// </summary>
        [Test]
        public void EnumerateWhenEmptyUniverseReturnsEmptyEnumeration()
        {
            A.CallTo(
                () => this.dataManifestInterpreter.PlayForward(A<TimeSpan>.Ignored))
                .Returns(Task.FromResult((IUniverse)new Universe(null)));

            var lazyCollection = new LazyTransientUniverse(this.dataManifestInterpreter);

            foreach (var _ in lazyCollection)
            {
                Assert.Fail("Should of been empty collection and not able to access enumeration");
            }

            Assert.True(true);
        }

        /// <summary>
        /// The enumerate when five events over two fetches enumerates five only.
        /// </summary>
        [Test]
        public void EnumerateWhenFiveEventsOverTwoFetchesEnumeratesFiveOnly()
        {
            var universeEventColl1 = new[]
             {
                 A.Fake<IUniverseEvent>(),
                 A.Fake<IUniverseEvent>(),
                 A.Fake<IUniverseEvent>(),
                 A.Fake<IUniverseEvent>(),
                 new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object())
             };

            A.CallTo(
                () => this.dataManifestInterpreter.PlayForward(A<TimeSpan>.Ignored))
                .Returns(Task.FromResult((IUniverse)new Universe(universeEventColl1)));

            var lazyCollection = new LazyTransientUniverse(
                this.dataManifestInterpreter);

            var tracker = 5;
            foreach (var _ in lazyCollection)
            {
                tracker--;
            }

            if (tracker != 0)
            {
                Assert.Fail("Enumerated over an unexpected number of items");
            }
            else
            {
                Assert.True(true);
            }
        }

        /// <summary>
        /// The enumerate when only genesis and eschaton enumerates twice only.
        /// </summary>
        [Test]
        public void EnumerateWhenOnlyGenesisAndEschatonEnumeratesTwiceOnly()
        {
            var universeEventColl = new[]
            {
                A.Fake<IUniverseEvent>(),
                new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object())
            };

            A.CallTo(
                () => this.dataManifestInterpreter.PlayForward(A<TimeSpan>.Ignored))
                .Returns(Task.FromResult((IUniverse)new Universe(universeEventColl)));

            var lazyCollection = new LazyTransientUniverse(this.dataManifestInterpreter);

            var tracker = 2;
            foreach (var _ in lazyCollection)
            {
                tracker--;
            }

            if (tracker != 0)
            {
                Assert.Fail("Enumerated over an unexpected number of items");
            }
            else
            {
                Assert.True(true);
            }
        }

        /// <summary>
        /// The enumerate when seven events over three fetches enumerates seven only.
        /// </summary>
        [Test]
        public void EnumerateWhenSevenEventsOverThreeFetchesEnumeratesSevenOnly()
        {
            var universeEventColl1 = new[] { A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>() };

            var universeEventColl2 = new[]
             {
                 A.Fake<IUniverseEvent>(), A.Fake<IUniverseEvent>(),
                 A.Fake<IUniverseEvent>()
             };

            var universeEventColl3 = new[]
             {
                 A.Fake<IUniverseEvent>(),
                 new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object())
             };

            var combinedUniverse = universeEventColl1.Concat(universeEventColl2).Concat(universeEventColl3);

            A.CallTo(
                () => this.dataManifestInterpreter.PlayForward(
                    A<TimeSpan>.Ignored)).
                Returns(Task.FromResult((IUniverse)new Universe(combinedUniverse)));

            var lazyCollection = new LazyTransientUniverse(this.dataManifestInterpreter);

            var tracker = 7;
            foreach (var _ in lazyCollection)
            {
                tracker--;
            }

            if (tracker != 0)
            {
                Assert.Fail("Enumerated over an unexpected number of items");
            }
            else
            {
                Assert.True(true);
            }
        }

        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.dataManifestInterpreter = A.Fake<IDataManifestInterpreter>();
        }
    }
}