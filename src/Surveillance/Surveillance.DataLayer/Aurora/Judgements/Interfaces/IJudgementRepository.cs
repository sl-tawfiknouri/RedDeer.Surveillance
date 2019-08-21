namespace Surveillance.DataLayer.Aurora.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    public interface IJudgementRepository
    {
        Task Save(IHighProfitJudgement highProfit);

        void Save(ICancelledOrderJudgement cancelledOrder);

        void Save(IHighVolumeJudgement highVolume);

        void Save(ILayeringJudgement layering);

        void Save(IMarkingTheCloseJudgement markingTheClose);

        void Save(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);

        void Save(IRampingJudgement ramping);

        void Save(ISpoofingJudgement spoofing);
    }
}