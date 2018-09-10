using Domain.Trades.Orders;
using System.Collections.Generic;
using System.Linq;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    public class Universe : IUniverse
    {
        public Universe(IReadOnlyCollection<TradeOrderFrame> trades)
        {
            Trades = trades ?? new List<TradeOrderFrame>();

            Setup();
        }

        /// <summary>
        /// We should do some work on initial set up to help prevent bugs
        /// In this case allow users to assume that trades are historically ordered
        /// </summary>
        private void Setup()
        {
            Trades = Trades.OrderBy(tra => tra.StatusChangedOn).ToList();
        }

        public IReadOnlyCollection<TradeOrderFrame> Trades { get; private set; }
    }
}