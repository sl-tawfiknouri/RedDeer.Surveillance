namespace Surveillance.Data.Universe.Lazy.Interfaces
{
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The LazyTransientUniverseFactory interface.
    /// Lazy - universe only exists as its observed
    /// transient - does not hold long lasting references to its contents
    /// </summary>
    public interface ILazyTransientUniverseFactory
    {
        /// <summary>
        /// The build universe function.
        /// as this is lazy it prepares the universe for building
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
        IUniverse Build(ScheduledExecution execution, ISystemProcessOperationContext operationContext);
    }
}