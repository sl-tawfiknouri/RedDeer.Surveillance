using System.Collections.Generic;
using Domain.Core.Financial.Assets;

namespace Domain.Core.Trading.Interfaces
{
    public interface IHoldings
    {
        IReadOnlyCollection<Holding> Holding { get; }

        Holding GetHolding(FinancialInstrument instrument);
    }
}