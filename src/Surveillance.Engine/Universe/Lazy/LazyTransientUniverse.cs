using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Lazy
{
    /// <summary>
    /// Both transient and lazy
    /// Only fetches what it needs as it needs
    /// Does not keep long lasting references to events
    /// Therefore this class should not be used in a multi threaded context
    /// Or called multiple times - populate another collection with the results
    /// If you wish to cache them but beware of memory consumption
    ///
    /// It only exists when you try to observe it
    /// </summary>
    public class LazyTransientUniverse : IEnumerable<IUniverseEvent>
    {
        private bool _hasEschatonOccurred = false;
        private bool _eschatonInBuffer = false;
        private bool _hasFetchedExecutions = false;

        private readonly ScheduledExecution _scheduledExecution;
        private readonly ILazyScheduledExecutioner _scheduledExecutioner;
        private readonly ISystemProcessOperationContext _opCtx;
        private readonly IUniverseBuilder _universeBuilder;
        private readonly Queue<IUniverseEvent> _buffer = new Queue<IUniverseEvent>();
        private Stack<ScheduledExecution> _executions = new Stack<ScheduledExecution>();

        public LazyTransientUniverse(
            ILazyScheduledExecutioner scheduledExecutioner,
            IUniverseBuilder universeBuilder,
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            _scheduledExecutioner = scheduledExecutioner ?? throw new ArgumentNullException(nameof(scheduledExecutioner));
            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
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
            GC.Collect();

            if (_eschatonInBuffer)
            {
                _hasEschatonOccurred = true;
                return;
            }

            var fetchGenesis = !_hasFetchedExecutions;
            if (!_hasFetchedExecutions)
            {
                _executions = _scheduledExecutioner.Execute(_scheduledExecution);
                _hasFetchedExecutions = true;
            }

            while (_executions.Any())
            {
                var exe = _executions.Pop();
                // the stack has simpler executions which already account for the leading/trailing edge of the universe
                var fetchEschaton = exe.TimeSeriesTermination >= _scheduledExecution.AdjustedTimeSeriesTermination;
                var universe = _universeBuilder.Summon(exe, _opCtx, fetchGenesis, fetchEschaton, _scheduledExecution.TimeSeriesInitiation, _scheduledExecution.TimeSeriesTermination).Result;

                foreach (var bufferedItem in universe.UniverseEvents)
                {
                    _buffer.Enqueue(bufferedItem);
                }

                _eschatonInBuffer = _eschatonInBuffer || fetchEschaton;

                if (universe.UniverseEvents.Any())
                    break;
            }
        }

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
                    if (_universe._buffer.Any())
                        _universe._buffer.Dequeue();
                }
                else
                {
                    // initial load
                    _universe.ReloadBuffer();
                    if (!_universe._buffer.Any())
                        return false;
                }

                if (_universe._eschatonInBuffer && !_universe._buffer.Any())
                {
                    _universe._hasEschatonOccurred = true;
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
