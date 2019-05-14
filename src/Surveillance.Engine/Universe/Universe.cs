using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class Universe : IUniverse
    {
        public Universe(IEnumerable<IUniverseEvent> universeEvents)
        {
            UniverseEvents = universeEvents ?? new List<IUniverseEvent>();

            Setup();
        }

        /// <summary>
        /// Initial set up to help prevent bugs
        /// In this case allow users to assume that trades are historically ordered
        /// </summary>
        private void Setup()
        {
            UniverseEvents = UniverseEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        public IEnumerable<IUniverseEvent> UniverseEvents { get; private set; }
    }
}