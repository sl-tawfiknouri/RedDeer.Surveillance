using FakeItEasy;
using NLog;
using NUnit.Framework;
using System;
using Domain.Streams;

namespace Domain.Tests.Streams
{
    [TestFixture]
    public class RecordingObserverTests
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
        }

        [Test]
        public void Constructor_NullILogger_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RecordingObserver<string>(null, 1));
        }

        [Test]
        public void OnError_LogsErrorMessage_Only()
        {
            var observer = new RecordingObserver<string>(_logger, 1);
            var exception = new Exception();

            observer.OnError(exception);

            A.CallTo(() => _logger.Log(LogLevel.Error, exception)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_LogsNullErrorMessage_WithSpecialText()
        {
            var observer = new RecordingObserver<string>(_logger, 1);

            observer.OnError(null);

            A.CallTo(() => _logger.Log(LogLevel.Error, "A null exception was passed to the recording observer")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Ctor_SetsIsCompletedFlagTo_False()
        {
            var observer = new RecordingObserver<string>(_logger, 1);

            Assert.IsFalse(observer.IsCompleted);
        }

        [Test]
        public void OnComplete_SetsIsCompletedFlagTo_True()
        {
            var observer = new RecordingObserver<string>(_logger, 1);

            observer.OnCompleted();

            Assert.IsTrue(observer.IsCompleted);
        }

        [Test]
        public void OnNext_AddsTick_ToBuffer()
        {
            var observer = new RecordingObserver<string>(_logger, 1);

            observer.OnNext("hello-world");
            var result = observer.Buffer.Remove();

            Assert.AreEqual(result, "hello-world");
        }
    }
}
