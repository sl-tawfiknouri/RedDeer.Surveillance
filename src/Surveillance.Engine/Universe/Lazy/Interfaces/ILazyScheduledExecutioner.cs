namespace Surveillance.Engine.Rules.Universe.Lazy.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    public interface ILazyScheduledExecutioner
    {
        Stack<ScheduledExecution> Execute(ScheduledExecution schedule);
    }
}