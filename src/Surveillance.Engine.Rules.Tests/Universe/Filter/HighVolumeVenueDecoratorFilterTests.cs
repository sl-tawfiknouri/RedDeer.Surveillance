using System;
using System.Collections.Concurrent;
using Domain.Surveillance.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    [TestFixture]
    public class HighVolumeVenueDecoratorFilterTests
    {
        private ILogger<IHighVolumeVenueFilter> _logger;
        private IUniverseFilterService _baseService;
        private IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<IHighVolumeVenueFilter>>();
            _baseService = A.Fake<IUniverseFilterService>();
            _universeUnsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
        }

        [Test]
        public void Constructor_NullTimeWindow_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new HighVolumeVenueDecoratorFilter(
                    null,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _logger));
        }

        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));

            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    null));
        }

        [Test]
        public void Subscribe_SubscribeObserver_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);
            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var result = venueFilter.Subscribe(anObserver);

            A.CallTo(() => 
                _universeUnsubscriberFactory
                    .Create(
                        A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                        A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Subscribe_SubscribeNullObserver_ReturnsNullAndDoesNotCallFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);

            var result = venueFilter.Subscribe(null);

            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustNotHaveHappened();

            Assert.IsNull(result);
        }

        [Test]
        public void Rule_DelegatesCallToDecoratee()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);

            var result = venueFilter.Rule;

            A.CallTo(() => _baseService.Rule).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Version_DelegatesCallToDecoratee()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);

            var result = venueFilter.Version;

            A.CallTo(() => _baseService.Version).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnCompleted_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);

            venueFilter.OnCompleted();
            
            A.CallTo(() => _baseService.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);
            var error = new Exception();

            venueFilter.OnError(error);

            A.CallTo(() => _baseService.OnError(error)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Subscribe_SubscribeObserverAndOnNextNull_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);
            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(null);
            
            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(null)).MustNotHaveHappened();
        }

        [Test]
        public void Subscribe_SubscribeObserverAndOnNextValid_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(timeWindows, _baseService, _universeUnsubscriberFactory, _logger);
            var anObserver = A.Fake<IObserver<IUniverseEvent>>();
            var onNext = A.Fake<IUniverseEvent>();

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(onNext);

            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(onNext)).MustHaveHappenedOnceExactly();
        }
    }
}
