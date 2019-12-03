using System.Threading.Tasks;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

namespace Surveillance.DataLayer.Aurora.Judgements.Interfaces
{
    public interface IJudgementRepository
    {
        Task SaveAsync(IHighProfitJudgement highProfit);

        Task SaveAsync(ICancelledOrderJudgement cancelledOrder);

        Task SaveAsync(IHighVolumeJudgement highVolume);

        Task SaveAsync(ILayeringJudgement layering);

        Task SaveAsync(IMarkingTheCloseJudgement markingTheClose);

        Task SaveAsync(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);

        Task SaveAsync(IRampingJudgement ramping);

        Task SaveAsync(ISpoofingJudgement spoofing);

        Task SaveAsync(IFixedIncomeHighProfitJudgement highProfit);

        Task SaveAsync(IFixedIncomeHighVolumeJudgement highVolume);
    }
}