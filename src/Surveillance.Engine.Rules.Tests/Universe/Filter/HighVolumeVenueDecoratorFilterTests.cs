using System;
using System.Collections.Concurrent;
using Domain.Surveillance.Streams.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    [TestFixture]
    public class HighVolumeVenueDecoratorFilterTests
    {
        private ILogger<HighVolumeVenueFilter> _logger;
        private IUniverseFilterService _baseService;
        private IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;
        private DecimalRangeRuleFilter _decimalRangeRuleFilter;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<HighVolumeVenueFilter>>();
            _baseService = A.Fake<IUniverseFilterService>();
            _universeUnsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            _decimalRangeRuleFilter = new DecimalRangeRuleFilter();
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
                    _decimalRangeRuleFilter,
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
                    _decimalRangeRuleFilter,
                    null));
        }

        [Test]
        public void Subscribe_SubscribeObserver_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = 
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

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
            var venueFilter = 
                new HighVolumeVenueDecoratorFilter(
                    timeWindows, 
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

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
            var venueFilter = 
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var result = venueFilter.Rule;

            A.CallTo(() => _baseService.Rule).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Version_DelegatesCallToDecoratee()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService, 
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var result = venueFilter.Version;

            A.CallTo(() => _baseService.Version).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnCompleted_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            venueFilter.OnCompleted();
            
            A.CallTo(() => _baseService.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var error = new Exception();

            venueFilter.OnError(error);

            A.CallTo(() => _baseService.OnError(error)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextNull_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

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
        public void OnNext_SubscribeObserverAndOnNextValid_ReturnsNonNullAndCallsFactory()
        {
            var baseDate = new DateTime(2018, 01, 01);
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var onNext1 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext1.EventTime).Returns(baseDate);

            var onNext2 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext2.EventTime).Returns(baseDate);
            A.CallTo(() => onNext2.StateChange).Returns(UniverseStateEvent.Eschaton);

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(onNext1);
            venueFilter.OnNext(onNext2);

            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext2)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextValidWaitsLengthOfWindow_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter =
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();
            var baseDate = new DateTime(2018, 01, 01);

            var onNext1 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext1.EventTime).Returns(baseDate);
            
            var onNext2 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext2.EventTime).Returns(baseDate.AddDays(1));

            var onNext3 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext3.EventTime).Returns(baseDate.AddDays(2));

            var onNext4 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext4.EventTime).Returns(baseDate.AddDays(3));

            var onNext5 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext5.EventTime).Returns(baseDate.AddDays(4.5));

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(onNext1);
            venueFilter.OnNext(onNext2);
            venueFilter.OnNext(onNext3);
            venueFilter.OnNext(onNext4);
            venueFilter.OnNext(onNext5);

            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext4)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext5)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextValidWaitsLengthOfWindowWithEschaton_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = 
                new HighVolumeVenueDecoratorFilter(
                    timeWindows,
                    _baseService,
                    _universeUnsubscriberFactory,
                    _decimalRangeRuleFilter,
                    _logger);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();
            var baseDate = new DateTime(2018, 01, 01);

            var onNext1 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext1.EventTime).Returns(baseDate);

            var onNext2 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext2.EventTime).Returns(baseDate.AddDays(1));

            var onNext3 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext3.EventTime).Returns(baseDate.AddDays(2));

            var onNext4 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext4.EventTime).Returns(baseDate.AddDays(3));
            A.CallTo(() => onNext4.StateChange).Returns(UniverseStateEvent.Eschaton);

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(onNext1);
            venueFilter.OnNext(onNext2);
            venueFilter.OnNext(onNext3);
            venueFilter.OnNext(onNext4);

            A.CallTo(() =>
                    _universeUnsubscriberFactory
                        .Create(
                            A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                            A<IObserver<IUniverseEvent>>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => anObserver.OnNext(onNext4)).MustHaveHappenedOnceExactly();
        }
    }
}
