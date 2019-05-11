using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    /// <summary>
    /// Both transient and lazy
    /// Only fetches what it needs as it needs
    /// Does not keep long lasting references to events
    /// Therefore this class should not be used in a multi threaded context
    /// Or called multiple times - populate another collection with the results
    /// if you wish to cache them but beware of memory consumption
    /// </summary>
    public class LazyTransientUniverse : IEnumerable<IUniverseEvent>
    {
        private bool _hasEschatonOccurred = false;
        private bool _eschatonInBuffer = false;

        private readonly ISystemProcessOperationContext _opCtx;
        private readonly ScheduledExecution _scheduledExecution;
        private readonly IUniverseBuilder _universeUniverseBuilder;
        private readonly Queue<IUniverseEvent> _buffer = new Queue<IUniverseEvent>();

        public LazyTransientUniverse(
            IUniverseBuilder universeBuilder,
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            _universeUniverseBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            _scheduledExecution = execution ?? throw new ArgumentNullException(nameof(execution));
            _opCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
        }

        public IEnumerator<IUniverseEvent> GetEnumerator()
        {
            return new LazyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ReloadBuffer()
        {
            if (_eschatonInBuffer)
            {
                _hasEschatonOccurred = true;
                return;
            }

            _buffer.Enqueue(new UniverseEvent(UniverseStateEvent.Order, DateTime.Now, "haha!"));

            _hasEschatonOccurred = true;
            _eschatonInBuffer = true;
        }

        // due to the transient property of this IEnumerable this is probably adding more complexity then it solves! May be better to implement direct in the lazyTransientUniverse
        private class LazyEnumerator : IEnumerator<IUniverseEvent>
        {
            private int _index = -1;
            private readonly LazyTransientUniverse _universe;

            public LazyEnumerator(LazyTransientUniverse universe)
            {
                _universe = universe ?? throw new ArgumentNullException(nameof(universe));
            }

            public bool MoveNext()
            {
                _index++;

                if (_index > 0)
                {
                    _universe._buffer.Dequeue();
                }

                return !_universe._hasEschatonOccurred;
            }

            public void Reset()
            {
                _index = -1;
            }

            public IUniverseEvent Current
            {
                get
                {
                    if (_universe._buffer.Any())
                    {
                        return _universe._buffer.Peek();
                    }

                    if (_universe._hasEschatonOccurred)
                    {
                        return null;
                    }

                    _universe.ReloadBuffer();

                    if (_universe._buffer.Any())
                    {
                        return _universe._buffer.Peek();
                    }

                    return null;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            { }
        }
    }
}
