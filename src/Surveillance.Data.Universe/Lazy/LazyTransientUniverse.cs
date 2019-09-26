namespace Surveillance.Data.Universe.Lazy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;
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
        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly Queue<IUniverseEvent> buffer = new Queue<IUniverseEvent>();

        /// <summary>
        /// The operation context.
        /// </summary>
        private readonly ISystemProcessOperationContext operationContext;

        /// <summary>
        /// The scheduled execution.
        /// </summary>
        private readonly ScheduledExecution scheduledExecution;

        /// <summary>
        /// The scheduled executioner.
        /// </summary>
        private readonly ILazyScheduledExecutioner scheduledExecutioner;

        /// <summary>
        /// The universe builder.
        /// </summary>
        private readonly IUniverseBuilder universeBuilder;

        /// <summary>
        /// The data manifest interpreter.
        /// </summary>
        private readonly IDataManifestInterpreter dataDataManifestInterpreter;

        /// <summary>
        /// The data manifest.
        /// </summary>
        private readonly IDataManifest dataManifest;

        /// <summary>
        /// The eschaton in buffer.
        /// </summary>
        private bool eschatonInBuffer;

        /// <summary>
        /// The has eschaton occurred.
        /// </summary>
        private bool hasEschatonOccurred;

        /// <summary>
        /// The has fetched executions.
        /// </summary>
        private bool hasFetchedExecutions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyTransientUniverse"/> class.
        /// </summary>
        /// <param name="scheduledExecutioner">
        /// The scheduled executioner.
        /// </param>
        /// <param name="universeBuilder">
        /// The universe builder.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="dataDataManifestInterpreter">
        /// The data manifest interpreter.
        /// </param>
        public LazyTransientUniverse(
            IUniverseBuilder universeBuilder,
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IDataManifestInterpreter dataDataManifestInterpreter)
        {
            this.universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this.scheduledExecution = execution ?? throw new ArgumentNullException(nameof(execution));
            this.operationContext = operationContext ?? throw new ArgumentNullException(nameof(operationContext));
            this.dataDataManifestInterpreter = dataDataManifestInterpreter ?? throw new ArgumentNullException(nameof(dataDataManifestInterpreter));
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<IUniverseEvent> GetEnumerator()
        {
            return new LazyEnumerator(this);
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The reload buffer.
        /// </summary>
        private void ReloadBuffer()
        {
            // trade off CPU for memory
            GC.Collect();

            if (this.eschatonInBuffer)
            {
                this.hasEschatonOccurred = true;
                return;
            }

            var universeTask = this.dataDataManifestInterpreter.PlayForward(TimeSpan.FromDays(7));
            var universe = universeTask.Result;

            foreach (var bufferedItem in universe.UniverseEvents)
            {
                if (bufferedItem.StateChange == UniverseStateEvent.Eschaton)
                {
                    this.eschatonInBuffer = true;
                }

                this.buffer.Enqueue(bufferedItem);
            }
        }

        /// <summary>
        /// The lazy enumerator.
        /// </summary>
        private class LazyEnumerator : IEnumerator<IUniverseEvent>
        {
            /// <summary>
            /// The universe.
            /// </summary>
            private readonly LazyTransientUniverse universe;

            /// <summary>
            /// The index.
            /// </summary>
            private int index = -1;

            /// <summary>
            /// Initializes a new instance of the <see cref="LazyEnumerator"/> class.
            /// </summary>
            /// <param name="universe">
            /// The universe.
            /// </param>
            public LazyEnumerator(LazyTransientUniverse universe)
            {
                this.universe = universe ?? throw new ArgumentNullException(nameof(universe));
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            public IUniverseEvent Current
            {
                get
                {
                    if (this.universe.buffer.Any())
                    {
                        return this.universe.buffer.Peek();
                    }

                    if (this.universe.hasEschatonOccurred)
                    {
                        return null;
                    }

                    this.universe.ReloadBuffer();

                    if (this.universe.buffer.Any())
                    {
                        return this.universe.buffer.Peek();
                    }

                    return null;
                }
            }

            /// <summary>
            /// The current.
            /// </summary>
            object IEnumerator.Current => this.Current;

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// The move next.
            /// </summary>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool MoveNext()
            {
                this.index++;

                if (this.index > 0)
                {
                    if (this.universe.buffer.Any())
                    {
                        this.universe.buffer.Dequeue();
                    }
                }
                else
                {
                    // initial load
                    this.universe.ReloadBuffer();
                    if (!this.universe.buffer.Any())
                    {
                        return false;
                    }
                }

                if (this.universe.eschatonInBuffer 
                    && !this.universe.buffer.Any())
                {
                    this.universe.hasEschatonOccurred = true;
                }

                return !this.universe.hasEschatonOccurred;
            }

            /// <summary>
            /// The reset.
            /// </summary>
            public void Reset()
            {
                this.index = -1;
            }
        }
    }
}