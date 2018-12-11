using System;
using Microsoft.Extensions.Logging;

namespace DomainV2.Streams
{
    /// <summary>
    /// This observer just records all the data it has recorded up to the maximum ring buffer allowance
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
            _logger.Log(LogLevel.Error, error.Message);
        }

        public void OnNext(T value)
        {
            Buffer.Add(value);
        }
    }
}
