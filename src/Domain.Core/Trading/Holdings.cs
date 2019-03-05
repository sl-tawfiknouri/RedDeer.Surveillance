using System.Collections.Generic;

namespace Domain.Core.Trading
{
    public class Holdings
    {
        public Holdings(IReadOnlyCollection<Holding> holding)
        {
            Holding = holding ?? new List<Holding>();
        }

        public IReadOnlyCollection<Holding> Holding { get; }
    }
}
