using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public class Holding
    {
        public Holding(FinancialInstrument instrument, Money averagePricePaid, Money totalPricePaid, long size)
        {
            Instrument = instrument;
            AveragePricePaid = averagePricePaid;
            TotalPricePaid = totalPricePaid;
            Size = size;
        }

        public FinancialInstrument Instrument { get; }
        public Money AveragePricePaid { get; }
        public Money TotalPricePaid { get; }
        public long Size { get; }
    }
}
