namespace Domain.Surveillance.Streams
{
    using System;

    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     This observer just records all the data it has recorded up to the maximum ring buffer allowance
    /// </summary>
    public class RecordingObserver<T> : IObserver<T>
        where T : class
    {
        private readonly ILogger _logger;

        public RecordingObserver(ILogger logger, int limit)
        {
            this.IsCompleted = false;
            this.Buffer = new RingBuffer<T>(limit);
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public RingBuffer<T> Buffer { get; }

        public bool IsCompleted { get; private set; }

        public void OnCompleted()
        {
            this.IsCompleted = true;
        }

        public void OnError(Exception error)
        {
            this._logger.LogError("RecordingObserver " + error.Message);
        }

        public void OnNext(T value)
        {
            this.Buffer.Add(value);
        }
    }
}