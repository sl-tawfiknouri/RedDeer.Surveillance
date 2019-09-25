namespace Surveillance.Engine.Rules.Universe.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

    /// <summary>
    /// The lazy transient universe factory.
    /// </summary>
    public class LazyTransientUniverseFactory : ILazyTransientUniverseFactory
    {
        /// <summary>
        /// The scheduled executioner.
        /// </summary>
        private readonly ILazyScheduledExecutioner scheduledExecutioner;

        /// <summary>
        /// The universe builder.
        /// </summary>
        private readonly IUniverseBuilder universeBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyTransientUniverseFactory"/> class.
        /// </summary>
        /// <param name="universeBuilder">
        /// The universe builder.
        /// </param>
        /// <param name="scheduledExecutioner">
        /// The scheduled executioner.
        /// </param>
        public LazyTransientUniverseFactory(
            IUniverseBuilder universeBuilder,
            ILazyScheduledExecutioner scheduledExecutioner)
        {
            this.universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this.scheduledExecutioner =
                scheduledExecutioner ?? throw new ArgumentNullException(nameof(scheduledExecutioner));
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
                this.scheduledExecutioner,
                this.universeBuilder,
                execution,
                operationContext);

            return new Universe(universeEvents);
        }
    }
}