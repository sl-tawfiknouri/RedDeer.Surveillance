namespace Surveillance.DataLayer.Aurora.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    /// <summary>
    /// The JudgementRepository interface.
    /// </summary>
    public interface IJudgementRepository
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highProfit">
        /// The high profit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Save(IHighProfitJudgement highProfit);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="cancelledOrder">
        /// The cancelled order.
        /// </param>
        void Save(ICancelledOrderJudgement cancelledOrder);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        void Save(IHighVolumeJudgement highVolume);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="layering">
        /// The layering.
        /// </param>
        void Save(ILayeringJudgement layering);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="markingTheClose">
        /// The marking the close.
        /// </param>
        void Save(IMarkingTheCloseJudgement markingTheClose);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        void Save(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="ramping">
        /// The ramping.
        /// </param>
        void Save(IRampingJudgement ramping);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="spoofing">
        /// The spoofing.
        /// </param>
        void Save(ISpoofingJudgement spoofing);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highProfit">
        /// The high profit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Save(IFixedIncomeHighProfitJudgement highProfit);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Save(IFixedIncomeHighVolumeJudgement highVolume);
    }
}