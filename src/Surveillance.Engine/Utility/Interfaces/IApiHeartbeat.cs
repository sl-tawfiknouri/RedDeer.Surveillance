namespace Surveillance.Engine.Rules.Utility.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The ApiHeartbeat interface.
    /// </summary>
    public interface IApiHeartbeat
    {
        /// <summary>
        /// The hearts beating.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> HeartsBeating();
    }
}