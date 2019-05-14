using System.Collections.Generic;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.Rules.Universe.Lazy.Interfaces
{
    public interface ILazyScheduledExecutioner
    {
        Stack<ScheduledExecution> Execute(ScheduledExecution schedule);
    }
}