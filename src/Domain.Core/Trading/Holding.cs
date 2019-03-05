using Domain.Core.Financial;

namespace Domain.Core.Trading
{
    public class Holding
    {
        public Holding(FinancialInstrument instrument, Money averagePricePaid, long size)
        {
            Instrument = instrument;
            AveragePricePaid = averagePricePaid;
            Size = size;
        }

        public FinancialInstrument Instrument { get; }
        public Money AveragePricePaid { get; }
        public long Size { get; }
    }
}
