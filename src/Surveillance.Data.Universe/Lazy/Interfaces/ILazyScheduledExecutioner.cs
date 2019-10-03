namespace Surveillance.Data.Universe.Lazy.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The LazyScheduledExecutioner interface.
    /// </summary>
    public interface ILazyScheduledExecutioner
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="schedule">
        /// The schedule.
        /// </param>
        /// <returns>
        /// The <see cref="Stack"/>.
        /// </returns>
        Stack<ScheduledExecution> Execute(ScheduledExecution schedule);
    }
}