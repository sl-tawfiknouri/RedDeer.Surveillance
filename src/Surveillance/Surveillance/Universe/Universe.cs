using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Trading;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    public class Universe : IUniverse
    {
        public Universe(
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<EquityIntraDayTimeBarCollection> marketEquityData,
            IReadOnlyCollection<EquityInterDayTimeBarCollection> interDayEquityData,
            IReadOnlyCollection<IUniverseEvent> universeEvents)
        {
            Trades = trades ?? new List<Order>();
            IntradayEquityData = marketEquityData ?? new List<EquityIntraDayTimeBarCollection>();
            InterDayEquityData = interDayEquityData ?? new List<EquityInterDayTimeBarCollection>();
            UniverseEvents = universeEvents ?? new List<IUniverseEvent>();

            Setup();
        }

        /// <summary>
        /// Initial set up to help prevent bugs
        /// In this case allow users to assume that trades are historically ordered
        /// </summary>
        private void Setup()
        {
            Trades = Trades.OrderBy(tra => tra.MostRecentDateEvent()).ToList();
            IntradayEquityData = IntradayEquityData.OrderBy(med => med.Securities.FirstOrDefault()?.TimeStamp).ToList();
            InterDayEquityData = InterDayEquityData.OrderBy(med => med.Securities.FirstOrDefault()?.TimeStamp).ToList();
            UniverseEvents = UniverseEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        public IReadOnlyCollection<Order> Trades { get; private set; }

        public IReadOnlyCollection<EquityIntraDayTimeBarCollection> IntradayEquityData { get; private set; }
        public IReadOnlyCollection<EquityInterDayTimeBarCollection> InterDayEquityData { get; private set; }
        public IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; private set; }
    }
}