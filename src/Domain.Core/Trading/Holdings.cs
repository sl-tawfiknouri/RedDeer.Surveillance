﻿using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Assets;

namespace Domain.Core.Trading
{
    public class Holdings : IHoldings
    {
        public Holdings(IReadOnlyCollection<Holding> holding)
        {
            Holding = holding ?? new List<Holding>();
        }

        public IReadOnlyCollection<Holding> Holding { get; }

        public Holding GetHolding(FinancialInstrument instrument)
        {
            return Holding?.FirstOrDefault(i => Equals(i.Instrument?.Identifiers, instrument?.Identifiers));
        }
    }
}
