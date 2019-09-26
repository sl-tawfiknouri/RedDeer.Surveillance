namespace Surveillance.Data.Universe.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;
    using Surveillance.Data.Universe.Lazy.Interfaces;

    /// <summary>
    /// The lazy transient universe factory.
    /// </summary>
    public class LazyTransientUniverseFactory : ILazyTransientUniverseFactory
    {
        /// <summary>
        /// The universe builder.
        /// </summary>
        private readonly IUniverseBuilder universeBuilder;

        /// <summary>
        /// The manifest interpreter.
        /// </summary>
        private readonly IDataManifestInterpreter _dataDataManifestInterpreter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyTransientUniverseFactory"/> class.
        /// </summary>
        /// <param name="universeBuilder">
        /// The universe builder.
        /// </param>
        /// <param name="dataDataManifestInterpreter">
        /// The data manifest interpreter
        /// </param>
        public LazyTransientUniverseFactory(
            IUniverseBuilder universeBuilder,
            IDataManifestInterpreter dataDataManifestInterpreter)
        {
            this.universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this._dataDataManifestInterpreter = dataDataManifestInterpreter ?? throw new ArgumentNullException(nameof(dataDataManifestInterpreter));
        }

        /// <summary>
        /// The build transient lazy universe.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverse"/>.
        /// </returns>
        public IUniverse Build(ScheduledExecution execution, ISystemProcessOperationContext operationContext)
        {
            var universeEvents = new LazyTransientUniverse(
                this.universeBuilder,
                execution,
                operationContext,
                this._dataDataManifestInterpreter);

            return new Universe(universeEvents);
        }
    }
}