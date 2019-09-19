namespace Surveillance.Engine.DataCoordinator.Coordinator.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The DataVerifier interface.
    /// </summary>
    public interface IDataVerifier
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