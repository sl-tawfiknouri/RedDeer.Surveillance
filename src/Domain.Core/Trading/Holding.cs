﻿using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public class Holding
    {
        public Holding(
            FinancialInstrument instrument,
            Money averagePricePaid,
            Money totalPricePaid,
            Money averagePriceSold,
            Money totalPriceSold,
            long size)
        {
            Instrument = instrument;
            AveragePricePaid = averagePricePaid;
            TotalPricePaid = totalPricePaid;
            AveragePriceSold = averagePriceSold;
            TotalPriceSold = totalPricePaid;
            Size = size;
        }

        public FinancialInstrument Instrument { get; }
        public Money AveragePricePaid { get; }
        public Money TotalPricePaid { get; }
        public Money AveragePriceSold { get; }
        public Money TotalPriceSold { get; }
        public long Size { get; }
    }
}
