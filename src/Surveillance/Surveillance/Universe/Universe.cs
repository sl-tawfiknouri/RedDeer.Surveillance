using Domain.Trades.Orders;
using System.Collections.Generic;
using System.Linq;
using Surveillance.Universe.Interfaces;
using Domain.Equity.Frames;

namespace Surveillance.Universe
{
    public class Universe : IUniverse
    {
        public Universe(
            IReadOnlyCollection<TradeOrderFrame> trades,
            IReadOnlyCollection<ExchangeFrame> marketEquityData)
        {
            Trades = trades ?? new List<TradeOrderFrame>();
            MarketEquityData = marketEquityData ?? new List<ExchangeFrame>();

            Setup();
        }

        /// <summary>
        /// We should do some work on initial set up to help prevent bugs
        /// In this case allow users to assume that trades are historically ordered
        /// </summary>
        private void Setup()
        {
            Trades = Trades.OrderBy(tra => tra.StatusChangedOn).ToList();
            MarketEquityData = MarketEquityData.OrderBy(med => med.Securities.FirstOrDefault()?.TimeStamp).ToList();
        }

        public IReadOnlyCollection<TradeOrderFrame> Trades { get; private set; }

        public IReadOnlyCollection<ExchangeFrame> MarketEquityData { get; private set; }
    }
}