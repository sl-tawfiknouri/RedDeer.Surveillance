using System;
using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Lazy
{
    public class LazyTransientUniverseFactory : ILazyTransientUniverseFactory
    {
        private readonly IUniverseBuilder _universeBuilder;
        private readonly ILazyScheduledExecutioner _scheduledExecutioner;

        public LazyTransientUniverseFactory(
            IUniverseBuilder universeBuilder,
            ILazyScheduledExecutioner scheduledExecutioner)
        {
            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            _scheduledExecutioner = scheduledExecutioner ?? throw new ArgumentNullException(nameof(scheduledExecutioner));
        }

        public IUniverse Build(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var universeEvents = new LazyTransientUniverse(
                _scheduledExecutioner,
                _universeBuilder,
                execution,
                opCtx);

            return new Universe(universeEvents);
        }
    }
}
