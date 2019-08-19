namespace Surveillance.Engine.Rules.Tests.Analytics.Streams
{
    using System;
    using System.Collections.Concurrent;

    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Analytics.Streams;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;

    [TestFixture]
    public class UniverseAlertStreamTests
    {
        private IUnsubscriberFactory<IUniverseAlertEvent> _factory;

        private ILogger<UniverseAlertStream> _logger;

        [Test]
        public void Add_Event_Does_Get_Passed_To_Observers()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();
            var alertEvent = A.Fake<IUniverseAlertEvent>();

            stream.Subscribe(observer);
            stream.Add(alertEvent);

            A.CallTo(() => observer.OnNext(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void Add_Event_Does_Get_Passed_To_Observers_Multiple_Times()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();
            var alertEvent = A.Fake<IUniverseAlertEvent>();

            stream.Subscribe(observer);
            stream.Add(alertEvent);
            stream.Add(alertEvent);
            stream.Add(alertEvent);

            A.CallTo(() => observer.OnNext(alertEvent)).MustHaveHappenedANumberOfTimesMatching(a => a == 3);
        }

        [Test]
        public void Add_Event_Does_Get_Passed_To_Observers_Twice_Times()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();
            var alertEvent1 = A.Fake<IUniverseAlertEvent>();
            var alertEvent2 = A.Fake<IUniverseAlertEvent>();

            stream.Subscribe(observer);
            stream.Add(alertEvent1);
            stream.Add(alertEvent2);

            A.CallTo(() => observer.OnNext(alertEvent1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => observer.OnNext(alertEvent2)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Add_Null_Event_Doesnt_Get_Passed_To_Observers()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();

            stream.Subscribe(observer);
            stream.Add(null);

            A.CallTo(() => observer.OnNext(A<IUniverseAlertEvent>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Constructor_Throws_For_Null_Logger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStream(this._factory, null));
        }

        [Test]
        public void Constructor_Throws_For_Null_Unsubscriber()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseAlertStream(null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._factory = A.Fake<IUnsubscriberFactory<IUniverseAlertEvent>>();
            this._logger = A.Fake<ILogger<UniverseAlertStream>>();
        }

        [Test]
        public void Subscribe_Calls_Unsubscriber_Create()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();

            stream.Subscribe(observer);

            A.CallTo(
                () => this._factory.Create(
                    A<ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>>>.Ignored,
                    A<IObserver<IUniverseAlertEvent>>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Subscribe_Calls_Unsubscriber_Create_Twice_For_Same_Observer()
        {
            var stream = this.Build();
            var observer = A.Fake<IObserver<IUniverseAlertEvent>>();

            stream.Subscribe(observer);
            stream.Subscribe(observer);

            A.CallTo(
                () => this._factory.Create(
                    A<ConcurrentDictionary<IObserver<IUniverseAlertEvent>, IObserver<IUniverseAlertEvent>>>.Ignored,
                    A<IObserver<IUniverseAlertEvent>>.Ignored)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void Subscribe_Null_Logger_Throws_Exception()
        {
            var stream = this.Build();

            Assert.Throws<ArgumentNullException>(() => stream.Subscribe(null));
        }

        private UniverseAlertStream Build()
        {
            return new UniverseAlertStream(this._factory, this._logger);
        }
    }
}