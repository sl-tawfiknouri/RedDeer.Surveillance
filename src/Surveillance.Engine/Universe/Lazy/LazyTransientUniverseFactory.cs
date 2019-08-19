namespace Surveillance.Engine.Rules.Universe.Lazy
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

    public class LazyTransientUniverseFactory : ILazyTransientUniverseFactory
    {
        private readonly ILazyScheduledExecutioner _scheduledExecutioner;

        private readonly IUniverseBuilder _universeBuilder;

        public LazyTransientUniverseFactory(
            IUniverseBuilder universeBuilder,
            ILazyScheduledExecutioner scheduledExecutioner)
        {
            this._universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this._scheduledExecutioner =
                scheduledExecutioner ?? throw new ArgumentNullException(nameof(scheduledExecutioner));
        }

        public IUniverse Build(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            var universeEvents = new LazyTransientUniverse(
                this._scheduledExecutioner,
                this._universeBuilder,
                execution,
                opCtx);

            return new Universe(universeEvents);
        }
    }
}