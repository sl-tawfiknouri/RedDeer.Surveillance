namespace Surveillance.Data.Universe
{
    using System.Collections.Generic;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The universe event comparer.
    /// </summary>
    public class UniverseEventComparer : IUniverseSortComparer
    {
        /// <summary>
        /// The priority.
        /// </summary>
        private readonly Dictionary<UniverseStateEvent, int> priority =
            new Dictionary<UniverseStateEvent, int>
             {
                 { UniverseStateEvent.Unknown, 0 },
                 { UniverseStateEvent.Genesis, 0 },
                 { UniverseStateEvent.EpochPrimordialUniverse, 1 },
                 { UniverseStateEvent.EpochRealUniverse, 1 },
                 { UniverseStateEvent.ExchangeOpen, 2 },
                 { UniverseStateEvent.EquityIntraDayTick, 3 },
                 { UniverseStateEvent.FixedIncomeIntraDayTick, 3 },
                 { UniverseStateEvent.EquityInterDayTick, 4 },
                 { UniverseStateEvent.FixedIncomeInterDayTick, 4 },
                 { UniverseStateEvent.EpochFutureUniverse, 4 },
                 { UniverseStateEvent.OrderPlaced, 5 },
                 { UniverseStateEvent.Order, 6 },
                 { UniverseStateEvent.OrderFilled, 7 },
                 { UniverseStateEvent.ExchangeClose, 8 },
                 { UniverseStateEvent.Eschaton, 9 }
             };

        /// <summary>
        /// A signed integer that indicates the relative values of x and y:
        /// If less than 0, x is less than y.
        /// If 0, x equals y.
        /// If greater than 0, x is greater than y.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(IUniverseEvent x, IUniverseEvent y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            if (x.EventTime > y.EventTime)
            {
                return 1;
            }

            if (x.EventTime < y.EventTime)
            {
                return -1;
            }

            var hasX = this.priority.ContainsKey(x.StateChange);
            var hasY = this.priority.ContainsKey(y.StateChange);

            if (!hasX && !hasY)
            {
                return 0;
            }

            if (!hasX)
            {
                return -1;
            }

            if (!hasY)
            {
                return 1;
            }

            var xPriority = this.priority[x.StateChange];
            var yPriority = this.priority[y.StateChange];

            if (xPriority == yPriority)
            {
                return 0;
            }

            if (xPriority < yPriority)
            {
                return -1;
            }

            if (xPriority > yPriority)
            {
                return 1;
            }

            return 0;
        }
    }
}