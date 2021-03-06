﻿namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.Universe.Filter;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    [TestFixture]
    public class HighVolumeVenueDecoratorFilterTests
    {
        private IUniverseFilterService _baseService;

        private IHighVolumeVenueFilter _highVolumeVenueFilter;

        private ILogger<HighVolumeVenueFilter> _logger;

        private IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;

        [Test]
        public void Constructor_NullTimeWindow_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new HighVolumeVenueDecoratorFilter(null, this._baseService, this._highVolumeVenueFilter));
        }

        [Test]
        public void OnCompleted_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            venueFilter.OnCompleted();

            A.CallTo(() => this._baseService.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_DelegatesCallToSubscriber()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var error = new Exception();

            venueFilter.OnError(error);

            A.CallTo(() => this._baseService.OnError(error)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextNull_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(null);

            A.CallTo(() => this._baseService.Subscribe(anObserver)).MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => anObserver.OnNext(null)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextValid_ReturnsNonNullAndCallsFactory()
        {
            var baseDate = new DateTime(2018, 01, 01);
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var onNext1 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext1.EventTime).Returns(baseDate);

            var onNext2 = A.Fake<IUniverseEvent>();
            A.CallTo(() => onNext2.EventTime).Returns(baseDate);
            A.CallTo(() => onNext2.StateChange).Returns(UniverseStateEvent.Eschaton);

            var result = venueFilter.Subscribe(anObserver);
            venueFilter.OnNext(onNext1);
            venueFilter.OnNext(onNext2);

            A.CallTo(() => this._baseService.Subscribe(anObserver)).MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => this._baseService.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext2)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_SubscribeObserverAndOnNextValidWaitsLengthOfWindow_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

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

            A.CallTo(() => this._baseService.Subscribe(anObserver)).MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => this._baseService.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext4)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext5)).MustNotHaveHappened();
        }

        [Test]
        public void
            OnNext_SubscribeObserverAndOnNextValidWaitsLengthOfWindowWithEschaton_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

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

            A.CallTo(() => this._baseService.Subscribe(anObserver)).MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);

            A.CallTo(() => this._baseService.OnNext(onNext1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._baseService.OnNext(onNext4)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Rule_DelegatesCallToDecoratee()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var result = venueFilter.Rule;

            A.CallTo(() => this._baseService.Rule).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<HighVolumeVenueFilter>>();
            this._baseService = A.Fake<IUniverseFilterService>();
            this._universeUnsubscriberFactory = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            this._highVolumeVenueFilter = A.Fake<IHighVolumeVenueFilter>();
        }

        [Test]
        public void Subscribe_SubscribeNullObserver_ReturnsNullAndDoesNotCallFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var result = venueFilter.Subscribe(null);

            A.CallTo(
                () => this._universeUnsubscriberFactory.Create(
                    A<ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>>.Ignored,
                    A<IObserver<IUniverseEvent>>.Ignored)).MustNotHaveHappened();

            Assert.IsNull(result);
        }

        [Test]
        public void Subscribe_SubscribeObserver_ReturnsNonNullAndCallsFactory()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var anObserver = A.Fake<IObserver<IUniverseEvent>>();

            var result = venueFilter.Subscribe(anObserver);

            A.CallTo(() => this._baseService.Subscribe(anObserver)).MustHaveHappenedOnceExactly();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Version_DelegatesCallToDecoratee()
        {
            var timeWindows = new TimeWindows("1", TimeSpan.FromDays(1));
            var venueFilter = new HighVolumeVenueDecoratorFilter(
                timeWindows,
                this._baseService,
                this._highVolumeVenueFilter);

            var result = venueFilter.Version;

            A.CallTo(() => this._baseService.Version).MustHaveHappenedOnceExactly();
        }
    }
}