using System.Threading.Tasks;
using Domain.Surveillance.Judgement.Equity;

namespace Surveillance.DataLayer.Aurora.Judgements.Interfaces
{
    public interface IJudgementRepository
    {
        Task Save(HighProfitJudgement highProfit);
        void Save(CancelledOrderJudgement cancelledOrder);
        void Save(HighVolumeJudgement highVolume);
        void Save(LayeringJudgement layering);
        void Save(MarkingTheCloseJudgement markingTheClose);
        void Save(PlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
        void Save(RampingJudgement ramping);
        void Save(SpoofingJudgement spoofing);
    }
}