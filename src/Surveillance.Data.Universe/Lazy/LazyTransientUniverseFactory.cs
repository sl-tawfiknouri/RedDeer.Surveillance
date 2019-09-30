namespace Surveillance.Data.Universe.Lazy
{
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
        /// Initializes a new instance of the <see cref="LazyTransientUniverseFactory"/> class.
        /// </summary>
        public LazyTransientUniverseFactory()
        {
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
        /// <param name="dataManifestInterpreter">
        /// The data Manifest Interpreter.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverse"/>.
        /// </returns>
        public IUniverse Build(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IDataManifestInterpreter dataManifestInterpreter)
        {
            var universeEvents = new LazyTransientUniverse(dataManifestInterpreter);

            return new Universe(universeEvents);
        }
    }
}