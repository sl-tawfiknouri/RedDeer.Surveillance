namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    /// <summary>
    /// The CancelledOrderJudgementService interface.
    /// </summary>
    public interface ICancelledOrderJudgementService
    {
        /// <summary>
        /// The judgement.
        /// </summary>
        /// <param name="cancelledOrder">
        /// The cancelled order.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Judgement(ICancelledOrderJudgement cancelledOrder);
    }
}