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
        /// The manifest interpreter.
        /// </summary>
        private readonly IDataManifestInterpreter dataDataManifestInterpreter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyTransientUniverseFactory"/> class.
        /// </summary>
        /// <param name="dataDataManifestInterpreter">
        /// The data manifest interpreter
        /// </param>
        public LazyTransientUniverseFactory(
            IDataManifestInterpreter dataDataManifestInterpreter)
        {
            this.dataDataManifestInterpreter =
                dataDataManifestInterpreter ?? throw new ArgumentNullException(nameof(dataDataManifestInterpreter));
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
            var universeEvents = new LazyTransientUniverse(this.dataDataManifestInterpreter);

            return new Universe(universeEvents);
        }
    }
}