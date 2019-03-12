using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Execution.Interfaces;

namespace Domain.Core.Trading
{
    public class SecurityExposure
    {
        public SecurityExposure(
            FinancialInstrument instrument,
            Money averagePricePaid,
            Money totalPricePaid,
            Money averagePriceSold,
            Money totalPriceSold,
            long size,
            IJudgement judgement)
        {
            Instrument = instrument;
            AveragePricePaid = averagePricePaid;
            TotalPricePaid = totalPricePaid;
            AveragePriceSold = averagePriceSold;
            TotalPriceSold = totalPricePaid;
            Size = size;
            Judgement = judgement;
        }

        public FinancialInstrument Instrument { get; }
        public Money AveragePricePaid { get; }
        public Money TotalPricePaid { get; }
        public Money AveragePriceSold { get; }
        public Money TotalPriceSold { get; }
        public long Size { get; }

        public IJudgement Judgement { get; }
    }
}
