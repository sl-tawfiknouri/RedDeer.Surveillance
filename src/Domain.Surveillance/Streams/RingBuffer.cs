

// ReSharper disable InconsistentlySynchronizedField
namespace Domain.Surveillance.Streams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RingBuffer<T>
        where T : class
    {
        private readonly int _limit;

        private readonly object _lock = new object();

        private readonly Queue<T> _queue;

        public RingBuffer(int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            this._limit = limit;
            this._queue = new Queue<T>();
        }

        public int Count => this._queue.Count;

        public void Add(T item)
        {
            lock (this._lock)
            {
                if (this._queue.Count >= this._limit) this._queue.Dequeue();

                this._queue.Enqueue(item);
            }
        }

        public T[] All()
        {
            return this._queue.ToArray();
        }

        public T Remove()
        {
            lock (this._lock)
            {
                if (this._queue.Any())
                    return this._queue.Dequeue();

                return null;
            }
        }
    }
}