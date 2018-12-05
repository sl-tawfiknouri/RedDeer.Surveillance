using Domain.Trades.Orders;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.Frames;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    public class Universe : IUniverse
    {
        public Universe(
            IReadOnlyCollection<TradeOrderFrame> trades,
            IReadOnlyCollection<ExchangeFrame> marketEquityData,
            IReadOnlyCollection<IUniverseEvent> universeEvents)
        {
            Trades = trades ?? new List<TradeOrderFrame>();
            MarketEquityData = marketEquityData ?? new List<ExchangeFrame>();
            UniverseEvents = universeEvents ?? new List<IUniverseEvent>();

            Setup();
        }

        /// <summary>
        /// Initial set up to help prevent bugs
        /// In this case allow users to assume that trades are historically ordered
        /// </summary>
        private void Setup()
        {
            Trades = Trades.OrderBy(tra => tra.StatusChangedOn).ToList();
            MarketEquityData = MarketEquityData.OrderBy(med => med.Securities.FirstOrDefault()?.TimeStamp).ToList();
            UniverseEvents = UniverseEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        public IReadOnlyCollection<TradeOrderFrame> Trades { get; private set; }

        public IReadOnlyCollection<ExchangeFrame> MarketEquityData { get; private set; }

        public IReadOnlyCollection<IUniverseEvent> UniverseEvents { get; private set; }
    }
}