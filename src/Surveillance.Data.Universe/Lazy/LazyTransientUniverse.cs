namespace Surveillance.Data.Universe.Lazy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy.Interfaces;

    /// <summary>
    ///     Both transient and lazy
    ///     Only fetches what it needs as it needs
    ///     Does not keep long lasting references to events
    ///     Therefore this class should not be used in a multi threaded context
    ///     Or called multiple times - populate another collection with the results
    ///     If you wish to cache them but beware of memory consumption
    ///     It only exists when you try to observe it
    /// </summary>
    public class LazyTransientUniverse : IEnumerable<IUniverseEvent>
    {
        private readonly Queue<IUniverseEvent> _buffer = new Queue<IUniverseEvent>();

        private readonly ISystemProcessOperationContext _opCtx;

        private readonly ScheduledExecution _scheduledExecution;

        private readonly ILazyScheduledExecutioner _scheduledExecutioner;

        private readonly IUniverseBuilder _universeBuilder;

        private bool _eschatonInBuffer;

        private Stack<ScheduledExecution> _executions = new Stack<ScheduledExecution>();

        private bool _hasEschatonOccurred;

        private bool _hasFetchedExecutions;

        public LazyTransientUniverse(
            ILazyScheduledExecutioner scheduledExecutioner,
            IUniverseBuilder universeBuilder,
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            this._scheduledExecutioner =
                scheduledExecutioner ?? throw new ArgumentNullException(nameof(scheduledExecutioner));
            this._universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this._scheduledExecution = execution ?? throw new ArgumentNullException(nameof(execution));
            this._opCtx = opCtx ?? throw new ArgumentNullException(nameof(opCtx));
        }

        public IEnumerator<IUniverseEvent> GetEnumerator()
        {
            return new LazyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void ReloadBuffer()
        {
            GC.Collect();

            if (this._eschatonInBuffer)
            {
                this._hasEschatonOccurred = true;
                return;
            }

            var fetchGenesis = !this._hasFetchedExecutions;
            if (!this._hasFetchedExecutions)
            {
                this._executions = this._scheduledExecutioner.Execute(this._scheduledExecution);
                this._hasFetchedExecutions = true;
            }

            while (this._executions.Any())
            {
                var exe = this._executions.Pop();

                // the stack has simpler executions which already account for the leading/trailing edge of the universe
                var fetchEschaton = exe.TimeSeriesTermination >= this._scheduledExecution.AdjustedTimeSeriesTermination;
                var universe = this._universeBuilder.Summon(
                    exe,
                    this._opCtx,
                    fetchGenesis,
                    fetchEschaton,
                    this._scheduledExecution.TimeSeriesInitiation,
                    this._scheduledExecution.TimeSeriesTermination).Result;

                foreach (var bufferedItem in universe.UniverseEvents) this._buffer.Enqueue(bufferedItem);

                this._eschatonInBuffer = this._eschatonInBuffer || fetchEschaton;

                if (universe.UniverseEvents.Any())
                    break;
            }
        }

        private class LazyEnumerator : IEnumerator<IUniverseEvent>
        {
            private readonly LazyTransientUniverse _universe;

            private int _index = -1;

            public LazyEnumerator(LazyTransientUniverse universe)
            {
                this._universe = universe ?? throw new ArgumentNullException(nameof(universe));
            }

            public IUniverseEvent Current
            {
                get
                {
                    if (this._universe._buffer.Any()) return this._universe._buffer.Peek();

                    if (this._universe._hasEschatonOccurred) return null;

                    this._universe.ReloadBuffer();

                    if (this._universe._buffer.Any()) return this._universe._buffer.Peek();

                    return null;
                }
            }

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                this._index++;

                if (this._index > 0)
                {
                    if (this._universe._buffer.Any()) this._universe._buffer.Dequeue();
                }
                else
                {
                    // initial load
                    this._universe.ReloadBuffer();
                    if (!this._universe._buffer.Any())
                        return false;
                }

                if (this._universe._eschatonInBuffer && !this._universe._buffer.Any())
                    this._universe._hasEschatonOccurred = true;

                return !this._universe._hasEschatonOccurred;
            }

            public void Reset()
            {
                this._index = -1;
            }
        }
    }
}