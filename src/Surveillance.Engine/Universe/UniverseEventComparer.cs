using System.Collections.Generic;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    public class UniverseEventComparer : IUniverseSortComparer
    {
        private readonly Dictionary<UniverseStateEvent, int> _priority =
            new Dictionary<UniverseStateEvent, int>
            {
                { UniverseStateEvent.Unknown, 0 },
                { UniverseStateEvent.Genesis, 1 },
                { UniverseStateEvent.ExchangeOpen, 2 },
                { UniverseStateEvent.EquityIntradayTick, 3 },
                { UniverseStateEvent.EquityInterDayTick, 4 },
                { UniverseStateEvent.OrderPlaced, 5 },
                { UniverseStateEvent.Order, 6 },
                { UniverseStateEvent.OrderFilled, 7 },
                { UniverseStateEvent.ExchangeClose, 8 },
                { UniverseStateEvent.Eschaton, 9 },
            };

        /*
            - A signed integer that indicates the relative values of x and y:
            - If less than 0, x is less than y.
            - If 0, x equals y.
            - If greater than 0, x is greater than y.
         */
        public int Compare(IUniverseEvent x, IUniverseEvent y)
        {
            if (x == null
                && y == null)
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

            var hasX = _priority.ContainsKey(x.StateChange);
            var hasY = _priority.ContainsKey(y.StateChange);

            if (!hasX
                && !hasY)
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

            var xPriority = _priority[x.StateChange];
            var yPriority = _priority[y.StateChange];

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
