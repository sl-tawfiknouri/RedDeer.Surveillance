namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The PlacingOrdersJudgementService interface.
    /// </summary>
    public interface IPlacingOrdersJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders);
    }
}