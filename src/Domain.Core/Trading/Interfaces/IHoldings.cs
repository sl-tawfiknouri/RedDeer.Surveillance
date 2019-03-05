using System.Collections.Generic;
using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public interface IHoldings
    {
        IReadOnlyCollection<Holding> Holding { get; }

        Holding GetHolding(FinancialInstrument instrument);
    }
}