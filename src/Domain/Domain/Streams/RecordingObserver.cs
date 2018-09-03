using System;
using NLog;

namespace Domain.Streams
{
    /// <summary>
    /// This observer just records all the data it has recorded upto the maximum ring buffer allowance
    /// </summary>
    public class RecordingObserver<T> : IObserver<T> where T : class
    {
        private readonly ILogger _logger;

        public RecordingObserver(ILogger logger, int limit)
        {
            IsCompleted = false;
            Buffer = new RingBuffer<T>(limit);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsCompleted { get; private set; }

        public RingBuffer<T> Buffer { get; }

        public void OnCompleted()
        {
            IsCompleted = true;
        }

        public void OnError(Exception error)
        {
            if (error == null)
            {
                _logger.Log(LogLevel.Error, "A null exception was passed to the recording observer");
                return;
            }

            _logger.Log(LogLevel.Error, error);
        }

        public void OnNext(T value)
        {
            Buffer.Add(value);
        }
    }
}
