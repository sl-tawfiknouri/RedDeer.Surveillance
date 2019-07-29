using System.Threading.Tasks;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;

namespace Surveillance.DataLayer.Aurora.Judgements.Interfaces
{
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