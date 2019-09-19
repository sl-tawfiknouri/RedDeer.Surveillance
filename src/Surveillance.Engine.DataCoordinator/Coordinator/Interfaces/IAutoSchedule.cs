namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The AutoSchedule interface.
    /// </summary>
    public interface IAutoSchedule
    {
        /// <summary>
        /// The scan.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Scan();
    }
}