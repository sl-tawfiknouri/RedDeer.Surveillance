using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Equity.Trading
{
    public class RingBuffer<T> : IEnumerable<T> where T: class
    {
        private int _limit;
        private Queue<T> _queue;
        private object _lock = new object();

        public RingBuffer(int limit)
        {
            if (limit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            _limit = limit;
            _queue = new Queue<T>();
        }

        public int Count { get { return _queue.Count(); } }

        public void Add(T item)
        {
            lock (_lock)
            {
                if (_queue.Count >= _limit)
                    _queue.Dequeue();

                _queue.Enqueue(item);
            }
        }

        public T Remove()
        {
            lock (_lock)
            {
                if (_queue.Any())
                    return _queue.Dequeue();

                return null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
