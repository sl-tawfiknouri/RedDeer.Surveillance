using Surveillance.Scheduler;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        /// <summary>
        /// Crack the cosmic egg and unscramble the reality
        /// </summary>
        public IUniverse Summon(ScheduledExecution execution)
        {
            if (execution == null)
            {
                return new Universe(null);
            }

            return new Universe(null);
        }
    }
}