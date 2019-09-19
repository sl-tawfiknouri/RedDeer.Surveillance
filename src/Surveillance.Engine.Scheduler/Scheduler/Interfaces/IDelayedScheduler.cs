namespace Surveillance.Engine.Scheduler.Scheduler.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The DelayedScheduler interface.
    /// </summary>
    public interface IDelayedScheduler
    {
        /// <summary>
        /// The schedule due tasks async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ScheduleDueTasksAsync();
    }
}