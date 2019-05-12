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
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(null)));

            var lazyCollection = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);
            
            foreach (var _ in lazyCollection)
            {
                Assert.Fail("Should of been empty collection and not able to access enumeration");
            }

            Assert.True(true);
        }

        [Test]
        public void Enumerate_WhenOnlyGenesisAndEschaton_EnumeratesTwiceOnly()
        {
            var execution = new ScheduledExecution();

            var universeEventColl = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };

            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl)));

            var lazyCollection = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);

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

        [Test]
        public void Enumerate_WhenFiveEvents_OverTwoFetches_EnumeratesFiveOnly()
        {
            var initialDate = new DateTimeOffset(2019, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var terminalDate = new DateTimeOffset(2019, 01, 09, 0, 0, 0, TimeSpan.Zero);
            var execution = new ScheduledExecution { TimeSeriesInitiation = initialDate, TimeSeriesTermination = terminalDate };

            var universeEventColl1 = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };

            var universeEventColl2 = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };
            
            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl1)));

            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);

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

        [Test]
        public void Enumerate_WhenSevenEvents_OverThreeFetches_EnumeratesSevenOnly()
        {
            var initialDate = new DateTimeOffset(2019, 01, 01, 0, 0, 0, TimeSpan.Zero);
            var terminalDate = new DateTimeOffset(2019, 01, 19, 0, 0, 0, TimeSpan.Zero);
            var execution = new ScheduledExecution { TimeSeriesInitiation = initialDate, TimeSeriesTermination = terminalDate };

            var universeEventColl1 = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };

            var universeEventColl2 = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };

            var universeEventColl3 = new[]
            {
                A.Fake<IUniverseEvent>(),
                A.Fake<IUniverseEvent>()
            };

            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    true,
                    false))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl1)));

            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    false))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl3)));

            A
                .CallTo(() => _universeBuilder.Summon(
                    A<ScheduledExecution>.Ignored,
                    A<ISystemProcessOperationContext>.Ignored,
                    false,
                    true))
                .Returns(Task.FromResult((IUniverse)new Engine.Rules.Universe.Universe(universeEventColl2)));

            var lazyCollection = new LazyTransientUniverse(_lazyScheduledExecutioner, _universeBuilder, execution, _opCtx);

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
    }
}
